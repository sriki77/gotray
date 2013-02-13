using GoTray.Common;
using GoTrayFeed;
using Windows.UI.Xaml.Navigation;

namespace GoTray
{
    public sealed partial class PipelineDetails : LayoutAwarePage
    {
        public PipelineDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var project = e.Parameter as GoProject;
            var pdm = new PiplineDetailsModel(project);
        }

        public class PiplineDetailsModel
        {
            public PiplineDetailsModel(GoProject project)
            {
                PipelineName = project.PipelineName;
                LastBuildTime = project.LastBuildTime;
            }

            public string PipelineName { get; private set; }
            public string[] Stages { get; private set; }
            public string LastBuildTime { get; private set; }

        }
    }
}