using System;
using System.Text.RegularExpressions;

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
        public Status Status { get; private set; }
        public string LastBuildLabel { get; set; }
        public string LastBuildTime { get; set; }
        public string WebUrl { get; set; }

        public bool Active
        {
            get { return "Building".Equals(Activity); }
        }


        public void DetermineStatusRelativeTo(Stage stage)
        {
            if (stage != null)
            {
                string curCounter = PipelineCounter(Name, WebUrl);
                string prevCounter = PipelineCounter(stage.Name, stage.WebUrl);
                if (!prevCounter.Equals(curCounter) || stage.Active)
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

        private string PipelineCounter(string name, string webUrl)
        {
            Match match = Regex.Match(webUrl, String.Format(@"/(\d+)/{0}/", name),
                                      RegexOptions.IgnoreCase);
            return match.Groups[1].Value;
        }
    }
}