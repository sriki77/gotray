using System;
using GoTray.Common;
using GoTrayFeed;
using GoTrayUtils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace GoTray
{
    public sealed class StageDetailsModel
    {
        public string StageName { get; internal set; }
        public bool StageActive { get; internal set; }

        private bool Equals(StageDetailsModel other)
        {
            return string.Equals(StageName, other.StageName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is StageDetailsModel && Equals((StageDetailsModel) obj);
        }

        public override int GetHashCode()
        {
            return StageName.GetHashCode();
        }
    }

    public sealed class PipelineDetailsModel
    {
        public PipelineDetailsModel()
        {
        }

        internal PipelineDetailsModel(GoProject project)
        {
            PipelineName = project.PipelineName;
            Status = project.Status;
            StageStatuses = new ProjectStatus[project.Stages.Count];
            Stage previous = null;
            for (int i = 0; i < StageStatuses.Length; ++i)
            {
                Stage curStage = project.Stages[i];
                ProjectStatus status = curStage.DetermineStageStatusRelativeTo(previous);
                if (status == ProjectStatus.None) break;
                StageStatuses[i] = status;
                previous = curStage;
            }
        }

        public string PipelineName { get; private set; }
        public ProjectStatus[] StageStatuses { get; private set; }
        public ProjectStatus Status { get; private set; }
    }

    public sealed partial class PipelineDetails : LayoutAwarePage
    {
        private readonly GoTrayConfiguration _config;

        public PipelineDetails()
        {
            InitializeComponent();
            _config = GoTrayConfiguration.TrayConfiguration;
            _config.ConfigChanged += ConfigurationChanged;
        }

        private void ConfigurationChanged(object sender, EventArgs e)
        {
            GoBack(this,null);
        }

        public PipelineDetailsModel PipelineDetailsStatus { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var project = e.Parameter as GoProject;
            PipelineDetailsStatus = new PipelineDetailsModel(project);
            DataContext = PipelineDetailsStatus;
        }


        private void GoToGitHub(object sender, RoutedEventArgs e)
        {
            TrayUtil.GoToGitHub();
        }
    }
}