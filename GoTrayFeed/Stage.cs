using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
[assembly: InternalsVisibleTo("GoTrayFeedTest")]
namespace GoTrayFeed
{
    public sealed class Stage
    {
        private string _stageName;

        public string Name
        {
            get { return _stageName; }
            set { _stageName = value.Split(':')[2].Trim(); }
        }

        public string Activity { get; set; }
        public string LastBuildStatus { get; set; }
        public Status Status { get; internal set; }
        public string LastBuildLabel { get; set; }
        public string LastBuildTime { get; set; }
        public string WebUrl { get; set; }
        internal string CurCounter;

        public bool Active
        {
            get { return "Building".Equals(Activity); }
        }


        public void DetermineStatusRelativeTo(string curRun)
        {
            PopulatePipelineCounter(Name, WebUrl);
            if (curRun != null)
            {
                if (!CurCounter.Equals(curRun))
                {
                    Status = Status.None;
                    return;
                }
            }
            if (Active)
            {
                Status = Status.Building;
                return;
            }

            Status = (Status) Enum.Parse(typeof (Status), LastBuildStatus, true);
        }

        private void PopulatePipelineCounter(string name, string webUrl)
        {
            Match match = Regex.Match(webUrl, String.Format(@"/(\d+)/{0}/", name),
                                      RegexOptions.IgnoreCase);
            CurCounter=match.Groups[1].Value;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "StageName: {0}, Name: {1}, Activity: {2}, LastBuildStatus: {3}, " +
                    "Status: {4}, LastBuildLabel: {5}, LastBuildTime: {6}, WebUrl: {7}, Active: {8}",
                    _stageName, Name, Activity, LastBuildStatus, Status, LastBuildLabel, LastBuildTime, WebUrl, Active);
        }
    }
}