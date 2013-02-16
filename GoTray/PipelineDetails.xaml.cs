using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoTray.Common;
using GoTrayFeed;
using GoTrayUtils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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
        private PipelineDetailsModel _pipelineDetailsModel;

        public PipelineDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var project = e.Parameter as GoProject;
            _pipelineDetailsModel = new PipelineDetailsModel(project);
            PipelineDetailsGrid.DataContext = _pipelineDetailsModel;
            StageGridView.ItemsSource = _pipelineDetailsModel.Stages;
            
        }

        private void UnpinStage(object sender, RoutedEventArgs e)
        {
            IList<object> selectedItems = StageGridView.SelectedItems;
            IList<StageDetailsModel> stages = new List<StageDetailsModel>(_pipelineDetailsModel.Stages);
            foreach (var item in selectedItems)
            {
                StageDetailsModel stage = item as StageDetailsModel;
                stages.Remove(stage);
            }
            StageGridView.ItemsSource = stages;
        }

        private void GoToGitHub(object sender, RoutedEventArgs e)
        {
            TrayUtil.GoToGitHub();
        }
    }
}