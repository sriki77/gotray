using System;
using System.Collections.Generic;

namespace GoTrayFeed
{
    public sealed class Pipeline
    {
        //<Project name="sadf :: defaultStage" activity="Building" lastBuildStatus="Success" 
        // lastBuildLabel="1" lastBuildTime="2012-11-15T14:58:58" 
        // webUrl="http://localhost:8153/go/pipelines/sadf/1/defaultStage/1"/>

        public List<Stage> Stages = new List<Stage>();
        private string _pipelineName;

        public string PipelineName
        {
            get { return _pipelineName; }
            set
            {
                PipelineFullName = value;
                _pipelineName = value.Split(':')[0].Trim();
            }
        }

        internal string PipelineFullName { get; private set; }

        public Status Status { get; private set; }

        public string LastBuildTime
        {
            get { return Stages[Stages.Count - 1].LastBuildTime; }
        }

        internal void DetermineStatus()
        {
            DetermineStageStatuses();
            Stage buildingStage = Stages.Find(stage => stage.Status == Status.Building);
            if (buildingStage != null)
            {
                Status = Status.Building;
                return;
            }

            Status = (Status) Enum.Parse(typeof (Status), Stages[Stages.Count - 1].LastBuildStatus, true);
        }

        private void DetermineStageStatuses()
        {
            Stage prev = null;
            foreach (Stage stage in Stages)
            {
                stage.DetermineStatusRelativeTo(prev);
                prev = stage;
            }
        }

        public void Merge(Pipeline other)
        {
            Stages.AddRange(other.Stages);
        }

        private bool Equals(Pipeline other)
        {
            return string.Equals(_pipelineName, other._pipelineName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Pipeline && Equals((Pipeline) obj);
        }

        public override int GetHashCode()
        {
            return _pipelineName.GetHashCode();
        }

        public override string ToString()
        {
            return
                string.Format(
                    "LastBuildTime: {0}, Status: {1}, PipelineFullName: {2}, PipelineName: {3}, PipelineName: {4}, Stages: {5}",
                    LastBuildTime, Status, PipelineFullName, PipelineName, _pipelineName, Stages.Count);
        }
    }
}