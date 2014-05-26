using System;
using System.Collections.Generic;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Content;

namespace SlxJobScheduler.Model
{
    [SDataPath("executions")]
    public class Execution
    {
        [SDataProtocolProperty]
        public string Key { get; set; }

        public SDataResource Job { get; set; }
        public SDataResource Trigger { get; set; }
        public SDataResource User { get; set; }
        public DateTime? ScheduledFireTimeUtc { get; set; }
        public DateTime? FireTimeUtc { get; set; }
        public string Phase { get; set; }
        public string PhaseDetail { get; set; }
        public decimal? Progress { get; set; }
        public TimeSpan? Elapsed { get; set; }
        public TimeSpan? Remaining { get; set; }
        public ExecutionStatus Status { get; set; }
        public object Result { get; set; }

        [JsonSimpleArray]
        public IList<State> State { get; set; }

        public bool Interrupt()
        {
            throw new NotSupportedException();
        }
    }

    public enum ExecutionStatus
    {
        Running,
        Complete,
        Interrupting,
        Interrupted,
        Error
    }
}