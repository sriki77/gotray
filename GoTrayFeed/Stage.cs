using System;

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
        public ProjectStatus StageStatus { get; private set; }
        public string LastBuildLabel { get; set; }
        public string LastBuildTime { get; set; }
        public string WebUrl { get; set; }

        public bool Active
        {
            get { return "Building".Equals(Activity); }
        }


        public ProjectStatus DetermineStageStatusRelativeTo(Stage stage)
        {

            if (stage != null)
            {
                string curLabel = StripReRunCount(LastBuildLabel);
                string prevLabel = StripReRunCount(stage.LastBuildLabel);
                if (!prevLabel.Equals(curLabel) || stage.Active)
                {
                    return ProjectStatus.None;
                }
            }
            return (ProjectStatus) Enum.Parse(typeof (ProjectStatus), LastBuildStatus, true);
        }

        private string StripReRunCount(string lastBuildLabel)
        {
            var splits = lastBuildLabel.Split(new[]{"::"}, StringSplitOptions.RemoveEmptyEntries);
            return splits[0].Trim();
        }
    }
}