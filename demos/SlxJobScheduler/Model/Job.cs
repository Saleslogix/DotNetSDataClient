using System;
using System.Collections.Generic;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Content;

namespace SlxJobScheduler.Model
{
    [SDataPath("jobs")]
    public class Job
    {
        private string _key;

        [SDataProtocolProperty]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        [SDataProtocolProperty]
        public string Descriptor { get; set; }

        public JobType Type { get; set; }
        public string Description { get; set; }

        [JsonSimpleArray]
        public IList<JobParameter> Parameters { get; set; }

        [JsonSimpleArray]
        public IList<State> State { get; set; }

        public string Trigger()
        {
            throw new NotSupportedException();
        }

        public bool Interrupt()
        {
            throw new NotSupportedException();
        }

        public void Pause()
        {
            throw new NotSupportedException();
        }

        public void Resume()
        {
            throw new NotSupportedException();
        }
    }

    public enum JobType
    {
        System,
        User
    }
}