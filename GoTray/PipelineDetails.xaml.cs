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
            StageStatuses = new GoProject.ProjectStatus[project.Stages.Count];
            for (int i = 0; i < StageStatuses.Length; ++i)
            {
                GoProject.ProjectStatus status = project.Stages[i].DetermineStageStatus();
                StageStatuses[i] = status;
                if (status == GoProject.ProjectStatus.Building) break;
            }
        }

        public string PipelineName { get; private set; }
        public GoProject.ProjectStatus[] StageStatuses { get; private set; }
        public GoProject.ProjectStatus Status { get; private set; }
    }

    public sealed partial class PipelineDetails : LayoutAwarePage
    {
        public PipelineDetails()
        {
            InitializeComponent();
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