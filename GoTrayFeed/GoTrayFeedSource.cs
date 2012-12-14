using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI.Popups;

namespace GoTrayFeed
{
    public sealed class GoTrayFeedSource
    {
        public readonly Task<IEnumerable<GoProject>> projects;

        public GoTrayFeedSource()
        {
            projects = PopulateProjectsAsync();
        }

        private async Task<IEnumerable<GoProject>> PopulateProjectsAsync()
        {
            const string serverUrl = "https://go01.thoughtworks.com/go/cctray.xml";
            ICredentials cred = new NetworkCredential("cruise_builder", "BXrQ51uhU");
            String responseXml = await RetrieveCcTrayResponseAsync(serverUrl, cred);
            XDocument cctrayXml = XDocument.Parse(responseXml);
            return (from lv1 in cctrayXml.Descendants("Project")
                    select new GoProject
                        {
                            ProjectName = lv1.Attribute("name").Value,
                            Activity = lv1.Attribute("activity").Value,
                            LastBuildLabel = lv1.Attribute("lastBuildLabel").Value,
                            LastBuildStatus = lv1.Attribute("lastBuildStatus").Value,
                            LastBuildTime = lv1.Attribute("lastBuildTime").Value,
                            WebUrl = lv1.Attribute("webUrl").Value
                        });
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