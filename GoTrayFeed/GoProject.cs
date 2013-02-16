using System;
using System.Collections.Generic;

namespace GoTrayFeed
{
    public sealed class GoProject
    {
        //<Project name="sadf :: defaultStage" activity="Building" lastBuildStatus="Success" lastBuildLabel="1" lastBuildTime="2012-11-15T14:58:58" webUrl="http://localhost:8153/go/pipelines/sadf/1/defaultStage/1"/>
        public List<Stage> Stages= new List<Stage>();
        private string _pipelineName;
        public enum ProjectStatus
        {
            Building,Success,Failure

        }

        public string PipelineName
        {
            get { return _pipelineName; }
            set
            {
                PipelineFullName=value; _pipelineName = value.Split(':')[0]; }
        }

        internal string PipelineFullName { get; private set; }

        public ProjectStatus Status
        {
            get { return DetermineProjectStatus(); }
            private set { throw new System.NotImplementedException(); }
        }

        private ProjectStatus DetermineProjectStatus()
        {
            Stage buildingStage = Stages.Find(stage => "Building".Equals(stage.Activity));
            if (buildingStage != null)
            {
                return ProjectStatus.Building;
            }

            return (ProjectStatus) Enum.Parse(typeof (ProjectStatus), Stages[Stages.Count - 1].LastBuildStatus,true);

        }

        public string LastBuildTime
        {
            get { return Stages[Stages.Count-1].LastBuildTime; }
        }

        public void Merge(GoProject other)
        {
            Stages.AddRange(other.Stages);
        }

        private bool Equals(GoProject other)
        {
            return string.Equals(_pipelineName, other._pipelineName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is GoProject && Equals((GoProject) obj);
        }

        public override int GetHashCode()
        {
            return _pipelineName.GetHashCode();
        }
    }

    public sealed class Stage
    {
        private string _stageName;
        public string Name {
            get { return _stageName; }
            set { _stageName = value.Split(':')[2].Trim(); }
        }
        public string Activity { get; set; }
        public string LastBuildStatus { get; set; }
        public string LastBuildLabel { get; set; }
        public string LastBuildTime { get; set; }
        public string WebUrl { get; set; }
        public bool Active {
            get { return "Building".Equals(Activity); }
        }
    }
}