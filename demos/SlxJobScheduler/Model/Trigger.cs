using System;
using System.Collections.Generic;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Content;

namespace SlxJobScheduler.Model
{
    [SDataPath("triggers")]
    public class Trigger
    {
        [SDataProtocolProperty]
        public string Key { get; set; }

        [SDataProtocolProperty]
        public string Descriptor { get; set; }

        public SDataResource Job { get; set; }
        public SDataResource User { get; set; }
        public DateTime? StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
        public int? RepeatCount { get; set; }
        public TimeSpan? RepeatInterval { get; set; }
        public string CronExpression { get; set; }
        public int? Priority { get; set; }
        public TriggerStatus Status { get; set; }
        public int? TimesTriggered { get; set; }
        public DateTime? PreviousFireTimeUtc { get; set; }
        public DateTime? NextFireTimeUtc { get; set; }

        [JsonSimpleArray]
        public IList<TriggerParameter> Parameters { get; set; }

        public void Pause()
        {
            throw new NotSupportedException();
        }

        public void Resume()
        {
            throw new NotSupportedException();
        }
    }

    public enum TriggerStatus
    {
        Normal,
        Paused,
        Complete,
        Error,
        Blocked,
        None
    }
}