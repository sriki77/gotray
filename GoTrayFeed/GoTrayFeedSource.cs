using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GoTrayFeed
{
    public sealed class GoTrayFeedSource
    {
        public readonly Task<IEnumerable<GoProject>> projects;

        public GoTrayFeedSource(string serverUrl, string userName, string password) : this()
        {
            if (String.IsNullOrEmpty(serverUrl)) return;
            projects = PopulateProjectsAsync(serverUrl, userName??"", password??"");
        }

        private GoTrayFeedSource()
        {
            TaskCompletionSource<IEnumerable<GoProject>> completionSource =
                new TaskCompletionSource<IEnumerable<GoProject>>();
            completionSource.SetResult(Enumerable.Empty<GoProject>());
            projects = completionSource.Task;
        }

        private async Task<IEnumerable<GoProject>> PopulateProjectsAsync(string serverUrl, string userName, string password)
        {
            ICredentials cred = new NetworkCredential(userName, password);
            String responseXml = await RetrieveCcTrayResponseAsync(serverUrl, cred);
            XDocument cctrayXml = XDocument.Parse(responseXml);
            IEnumerable<GoProject> goProjects = (from lv1 in cctrayXml.Descendants("Project")
                                                 select new GoProject
                                                     {
                                                         PipelineName = lv1.Attribute("name").Value,
                                                         Stages= new []{
                                                             new Stage
                                                                 {
                                                                     Activity = lv1.Attribute("activity").Value,
                                                                     LastBuildLabel = lv1.Attribute("lastBuildLabel").Value,
                                                                     LastBuildStatus = lv1.Attribute("lastBuildStatus").Value,
                                                                     LastBuildTime = lv1.Attribute("lastBuildTime").Value,
                                                                     WebUrl = lv1.Attribute("webUrl").Value
                                                                 }}.ToList()
                                                     });
            return Sanitize(goProjects);
        }

        private IEnumerable<GoProject> Sanitize(IEnumerable<GoProject> goProjects)
        {
            List<GoProject> projects=new List<GoProject>();
            foreach (var project in goProjects)
            {
                if (IsJob(project)) continue;
                GoProject projectToFind = project;
                GoProject foundProject = projects.Find((p) => p.PipelineName.Equals(projectToFind.PipelineName));
                if (foundProject != null)
                {
                    foundProject.Merge(projectToFind);
                }
                else
                {
                    projects.Add(project);
                }

            }
            return projects;
        }

        private bool IsJob(GoProject project)
        {
            return project.PipelineName.IndexOf("::", System.StringComparison.Ordinal) != 
                project.PipelineName.LastIndexOf("::", System.StringComparison.Ordinal);
        }

        private async Task<string> RetrieveCcTrayResponseAsync(string serverUrl, ICredentials credential)
        {
            var handler = new HttpClientHandler();
            handler.Credentials = credential;
            handler.AllowAutoRedirect = true;
            var client = new HttpClient(handler);
            HttpResponseMessage response = await client.GetAsync(serverUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}