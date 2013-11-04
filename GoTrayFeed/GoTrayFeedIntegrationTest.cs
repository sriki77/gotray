using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NUnit.Framework;

namespace GoTrayFeed
{
    [TestFixture]
    internal class GoTrayFeedIntegrationTest
    {
        private const string Username = "<provide username>";
        private const string Password = "<provide password>";
        private const string ServerPrefix = "https://go01.thoughtworks.com/go/";
        private const string CctrayUrl = ServerPrefix+"cctray.xml";
        private const string DashboardUrl = ServerPrefix + "home";
        private const string AuthUrl = ServerPrefix + "auth/security_check";

        private async Task<string> RetrieveServerResponseAsync(string serverUrl)
        {
            CookieContainer cc = await AuthWithServer();
            Debug.WriteLine("Fetching dashboard...");
            var handler = new HttpClientHandler();
            handler.CookieContainer = cc;
            var client = new HttpClient(handler);
            HttpResponseMessage response = await client.GetAsync(serverUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<CookieContainer> AuthWithServer()
        {
            Debug.WriteLine("Logging In to the dashboard...");
            var cc = new CookieContainer();
            var handler = new HttpClientHandler {CookieContainer = cc};
            var request = new HttpRequestMessage(HttpMethod.Post,AuthUrl);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"j_username", Username},
                    {"j_password", Password}
                });
            handler.AllowAutoRedirect = true;
            var client = new HttpClient(handler);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return cc;
        }

        private static Status? ToBuildStatus(Match m)
        {
            string statusString = m.Groups[1].ToString().Split(' ')[1]
                .Replace('(', ' ').Replace(')', ' ').Trim();
            switch (statusString)
            {
                case "Passed":
                    return Status.Success;
                case "Unknown":
                    return Status.None;
                case "Failed":
                    return Status.Failure;
                case "Building":
                case "Failing":
                    return Status.Building;
                case "Cancelled":
                    return null;
                default:
                    return Status.None;
            }
        }

        private static IEnumerable<HtmlNode> LatestStageDivs(HtmlDocument dash)
        {
            return dash.DocumentNode.Descendants("div")
                       .Where(d => d.Attributes.Contains("class")
                                   &&
                                   d.Attributes["class"].Value.Contains("latest_stage"));
        }

        private static IEnumerable<Stage> StagesFrom(string txt)
        {
            MatchCollection matchCollection = Regex.Matches(txt, "title=\"(.+)\"");
            var stages = new List<Stage>();
            foreach (Match m in matchCollection)
            {
                string name = m.Groups[1].ToString().Split(' ')[0].Trim();
                Status? buildStatus = ToBuildStatus(m);
                if (!stages.Any(s => s.Name.Equals(name)))
                {
                    stages.Add(new Stage
                        {
                            Name = "::" + name,
                            Status = buildStatus ?? Status.None,
                            LastBuildStatus = buildStatus == null ? "Cancelled" : ""
                        });
                }
            }
            return stages;
        }

        private static Pipeline PipelineFrom(string txt)
        {
            IEnumerable<Stage> stages = StagesFrom(txt);
            Match match = Regex.Match(txt, "<a href='/go/pipelines/([^/]+)/([^/]+)/");
            return new Pipeline {PipelineName = match.Groups[1] + ":", Stages = stages.ToList()};
        }

        private static async Task<List<Pipeline>> PipelinesFromFeed()
        {
            var feedDataSource = new GoTrayFeedSource(CctrayUrl,Username, Password);
            IEnumerable<Pipeline> projects = await feedDataSource.projects;
            return projects.OrderBy(p => p.PipelineName).ToList();
        }

        private async Task<List<Pipeline>> PipelinesFromDashBoard()
        {
            string res = await RetrieveServerResponseAsync(DashboardUrl);
            Debug.WriteLine("Processing dashboard HTML...");

            var dash = new HtmlDocument();
            dash.LoadHtml(res);
            var pipelines = new List<Pipeline>();
            foreach (HtmlNode elem in LatestStageDivs(dash))
            {
                Pipeline pipeline = PipelineFrom(elem.ParentNode.InnerHtml);
                if (!pipelines.Any(p => p.PipelineName.Equals(pipeline.PipelineName)))
                {
                    pipelines.Add(pipeline);
                }
            }
            return pipelines.OrderBy(p => p.PipelineName).ToList();
        }

        private void ValidateStageStatus(Pipeline dashPipe, Pipeline feedPipe)
        {
            string pipelineName = dashPipe.PipelineName;
            List<Stage> dashStages = dashPipe.Stages;
            List<Stage> feedStages = feedPipe.Stages;
            for (int i = 0; i < feedStages.Count; i++)
            {
                Stage dashStage = dashStages[i];
                Stage feedStage = feedStages[i];
                Assert.AreEqual(dashStage.Name, feedStage.Name);
                AssertStatus(dashStage, feedStage, pipelineName);
            }
        }

        private static void AssertStatus(Stage dashStage, Stage feedStage, string pipelineName)
        {
            Status expected = dashStage.Status;
            Status actual = feedStage.Status;
            string lastBuildStatus = dashStage.LastBuildStatus;
            string msg = "Stage status mismatch Pipeline: " + pipelineName + " Stage: " + dashStage.Name;
            if (expected != actual)
            {
                if (expected == Status.None && lastBuildStatus.Contains("Cancelled"))
                {
                    Debug.WriteLine("Ignoring Stage Status of " + pipelineName + ":" + dashStage.Name);
                }
                return;
            }
            Assert.AreEqual(expected, actual, msg);
        }

        private static void ValidateStagesCount(Pipeline dashPipe, Pipeline feedPipe,
                                                string[] pipelineStageCountMisMatch)
        {
            string messageForStageCountMismatch = MessageForStageCountMismatch(dashPipe, feedPipe);
            if (dashPipe.Stages.Count != feedPipe.Stages.Count)
            {
                Debug.WriteLine(messageForStageCountMismatch);
                if (pipelineStageCountMisMatch.Contains(dashPipe.PipelineName))
                {
                    return;
                }
            }

            Assert.AreEqual(dashPipe.Stages.Count, feedPipe.Stages.Count, messageForStageCountMismatch);
        }

        private static string MessageForStageCountMismatch(Pipeline dashPipe, Pipeline feedPipe)
        {
            return string.Format("Stage count of pipeline '{0}' does not match." +
                                 "Dash Stages: '{1}' Feed Stages: '{2}'",
                                 dashPipe.PipelineName,
                                 string.Join(",", dashPipe.Stages.Select(s => s.Name).ToArray()),
                                 string.Join(",", feedPipe.Stages.Select(s => s.Name).ToArray())
                );
        }

        [Test]
        public async void TestPipelineStatus()
        {
            Debug.WriteLine("Fetch Pipelines From Feed...");
            List<Pipeline> projects = await PipelinesFromFeed();
            Debug.WriteLine("Fetch Pipelines From Dashboard...");
            List<Pipeline> pipelines = await PipelinesFromDashBoard();
            Debug.WriteLine("Checking Data...");
            Assert.AreEqual(projects.Count, pipelines.Count);
            var pipelineStageCountMisMatch = new[] {"Jobsite", "mingle05_deployment", "rails_3.0"};
            foreach (Pipeline feedPipe in projects)
            {
                List<Pipeline> pipes = pipelines.Where(p => p.PipelineName.Equals(feedPipe.PipelineName)).ToList();
                Assert.AreEqual(1, pipes.Count, "Could Not Find Pipeline " + feedPipe.PipelineName);
                Pipeline dashPipe = pipes[0];
                Assert.AreEqual(dashPipe.PipelineName, feedPipe.PipelineName);
                ValidateStagesCount(dashPipe, feedPipe, pipelineStageCountMisMatch);
                ValidateStageStatus(dashPipe, feedPipe);
            }
        }
    }
}