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
        public readonly Task<IEnumerable<Pipeline>> projects;

        public GoTrayFeedSource(string serverUrl, string userName, string password) : this()
        {
            if (String.IsNullOrEmpty(serverUrl)) return;
            projects = PopulateProjectsAsync(serverUrl, userName ?? "", password ?? "");
        }

        private GoTrayFeedSource()
        {
            var completionSource =
                new TaskCompletionSource<IEnumerable<Pipeline>>();
            completionSource.SetResult(Enumerable.Empty<Pipeline>());
            projects = completionSource.Task;
        }

        private async Task<IEnumerable<Pipeline>> PopulateProjectsAsync(string serverUrl, string userName,
                                                                        string password)
        {
            ICredentials cred = new NetworkCredential(userName, password);
            String responseXml = await RetrieveCcTrayResponseAsync(serverUrl, cred);
            XDocument cctrayXml = XDocument.Parse(responseXml);
            IEnumerable<Pipeline> goProjects = (from lv1 in cctrayXml.Descendants("Project")
                                                select new Pipeline
                                                    {
                                                        PipelineName = lv1.Attribute("name").Value,
                                                        Stages = new[]
                                                            {
                                                                new Stage
                                                                    {
                                                                        Name = lv1.Attribute("name").Value,
                                                                        Activity = lv1.Attribute("activity").Value,
                                                                        LastBuildLabel =
                                                                            lv1.Attribute("lastBuildLabel").Value,
                                                                        LastBuildStatus =
                                                                            lv1.Attribute("lastBuildStatus").Value,
                                                                        LastBuildTime =
                                                                            lv1.Attribute("lastBuildTime").Value,
                                                                        WebUrl = lv1.Attribute("webUrl").Value
                                                                    }
                                                            }.ToList()
                                                    });
            return Sanitize(goProjects);
        }

        private IEnumerable<Pipeline> Sanitize(IEnumerable<Pipeline> goPipelines)
        {
            var pipelines = new List<Pipeline>();
            foreach (Pipeline pipeline in goPipelines)
            {
                if (IsJob(pipeline)) continue;
                Pipeline projectToFind = pipeline;
                Pipeline foundPipeline = pipelines.Find((p) => p.PipelineName.Equals(projectToFind.PipelineName));
                if (foundPipeline != null)
                {
                    foundPipeline.Merge(projectToFind);
                }
                else
                {
                    pipelines.Add(pipeline);
                }
            }

            return CalculatePipelineStatuses(pipelines);
        }

        private IEnumerable<Pipeline> CalculatePipelineStatuses(List<Pipeline> pipelines)
        {
            foreach (Pipeline pipeline in pipelines)
            {
                pipeline.DetermineStatus();
            }
            return pipelines;
        }


        private bool IsJob(Pipeline pipeline)
        {
            return pipeline.PipelineFullName.IndexOf("::", StringComparison.Ordinal) !=
                   pipeline.PipelineFullName.LastIndexOf("::", StringComparison.Ordinal);
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