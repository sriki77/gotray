﻿using System.Collections.Generic;

namespace GoTrayFeed
{
    public sealed class GoProject
    {
        //<Project name="sadf :: defaultStage" activity="Building" lastBuildStatus="Success" lastBuildLabel="1" lastBuildTime="2012-11-15T14:58:58" webUrl="http://localhost:8153/go/pipelines/sadf/1/defaultStage/1"/>
        public List<Stage> Stages= new List<Stage>();
        private string _pipelineName;

        public string PipelineName
        {
            get { return _pipelineName; }
            set { _pipelineName = value.Split(':')[0]; }
        }

        public string LastBuildTime
        {
            get { return Stages[Stages.Count-1].LastBuildTime; }
        }

        public void Merge(GoProject other)
        {
            Stages.AddRange(other.Stages);
        }
    }

    public sealed class Stage
    {
        public string Name { get; set; }
        public string Activity { get; set; }
        public string LastBuildStatus { get; set; }
        public string LastBuildLabel { get; set; }
        public string LastBuildTime { get; set; }
        public string WebUrl { get; set; }
    }
}