using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace GoTrayFeed
{
    [TestFixture]
    internal class GoTrayFeedIntegrationTest
    {
        [Test]
        public async void TestPipelineStatus()
        {
            ICredentials cred = new NetworkCredential();
            string responseXml =
                await RetrieveServerResponseAsync("https://go01.thoughtworks.com/go/api/pipelines.xml", cred);
            XDocument feed = XDocument.Parse(responseXml);
            XNamespace ns = "http://www.w3.org/2005/Atom";
            var stages = from lvl in feed.Descendants("pipeline") select lvl.Attribute("href").Value;
            foreach (var stage in stages)
            {
                responseXml =
                await RetrieveServerResponseAsync(stage, cred);
                feed = XDocument.Parse(responseXml);
                string stageLink = feed.Descendants(ns + "entry").Descendants(ns + "link").First().Attribute("href").Value;
                responseXml =
                await RetrieveServerResponseAsync(stageLink, cred);
                feed = XDocument.Parse(responseXml);
                var stageStatus = feed.Descendants("result").First().Value;
            }
        }

        private async Task<string> RetrieveServerResponseAsync(string serverUrl, ICredentials credential)
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