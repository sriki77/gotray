using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;

namespace GoTray
{
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }



        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            String responseXml = await retrieveCCTrayResponse();
            XDocument cctrayXml = XDocument.Parse(responseXml);
            var allProjects = from lv1 in cctrayXml.Descendants("Project")
                              select new
                              {
                                  ProjectName = lv1.Attribute("name").Value,
                                  Activity = lv1.Attribute("activity").Value
                              };

//            XmlDocument xmltile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText01);
//            xmltile.GetElementsByTagName("text")[0].AppendChild(xmltile.CreateTextNode((i++).ToString()));
//            TileNotification tileUpdate = new TileNotification(xmltile);
//            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileUpdate);
        }

        private static async Task<string> retrieveCCTrayResponse()
        {
            String responseXml = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8153/go/cctray.xml");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                responseXml = reader.ReadToEnd();
            }
            return responseXml;
        }

    }
}
