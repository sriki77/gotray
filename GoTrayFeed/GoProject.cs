namespace GoTrayFeed
{
    public sealed class GoProject
    {
        //<Project name="sadf :: defaultStage" activity="Building" lastBuildStatus="Success" lastBuildLabel="1" lastBuildTime="2012-11-15T14:58:58" webUrl="http://localhost:8153/go/pipelines/sadf/1/defaultStage/1"/>
        public string ProjectName { get; set; }
        public string Activity { get; set; }
        public string LastBuildStatus { get; set; }
        public string LastBuildLabel { get; set; }
        public string LastBuildTime { get; set; }
        public string WebUrl { get; set; }
    }
}