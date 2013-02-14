using GoTray.Common;
using GoTrayFeed;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace GoTray
{
    public sealed class StageDetailsModel
    {
        public string StageName { get; internal set; }
        public bool StageActive { get; internal set; }
    }

    public sealed class PipelineDetailsModel
    {
        public PipelineDetailsModel()
        {
            
        }

        internal PipelineDetailsModel(GoProject project)
        {
            PipelineName = project.PipelineName;
            LastBuildTime = project.LastBuildTime;
            Stages=new StageDetailsModel[project.Stages.Count];
            for (int i = 0; i < Stages.Length; ++i)
            {
                Stages[i] = new StageDetailsModel
                    {
                        StageName = project.Stages[i].Name,
                        StageActive = project.Stages[i].Active,
                    };
            }
        }

        public string PipelineName { get; private set; }
        public StageDetailsModel[] Stages { get; private set; }
        public string LastBuildTime { get; private set; }

    }




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
            PipelineDetailsModel model = new PipelineDetailsModel(project);
            PipelineDetailsGrid.DataContext = model;
            StageGridView.ItemsSource = model.Stages;
            
        }

    }
}