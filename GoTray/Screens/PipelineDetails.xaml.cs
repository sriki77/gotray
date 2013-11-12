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
        public Status Status { get; internal set; }

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

        internal PipelineDetailsModel(Pipeline pipeline)
        {
            PipelineName = pipeline.PipelineName;
            Status = pipeline.Status;
            StageDetails = new StageDetailsModel[pipeline.Stages.Count];
            for (int i = 0; i < StageDetails.Length; ++i)
            {
                StageDetails[i] = new StageDetailsModel
                    {
                        Status = pipeline.Stages[i].Status,
                        StageName = pipeline.Stages[i].Name
                    };
            }
            LastUpdatedTime = "Last Updated: " + DateTime.Now.ToString("HH:mm:ss tt");
        }

        public string PipelineName { get; private set; }
        public StageDetailsModel[] StageDetails { get; private set; }
        public Status Status { get; private set; }
        public string LastUpdatedTime { get; private set; }
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

        public PipelineDetailsModel PipelineDetailsStatus { get; private set; }

        private void ConfigurationChanged(object sender, EventArgs e)
        {
            GoBack(this, null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PipelineDetailsStatus = new PipelineDetailsModel(e.Parameter as Pipeline);
            DataContext = PipelineDetailsStatus;
        }
    }
}