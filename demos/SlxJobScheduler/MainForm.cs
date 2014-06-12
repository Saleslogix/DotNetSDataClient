using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Saleslogix.SData.Client;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Linq;
using SlxJobScheduler.Model;
using SlxJobScheduler.Properties;

namespace SlxJobScheduler
{
    public partial class MainForm : Form
    {
        private readonly ICredentials _credentials = new PromptCredentials();
        private ISDataClient _client;

        public MainForm()
        {
            InitializeComponent();

            TypeDescriptor.AddProvider(new TriggerDescriptionProvider(), typeof (Trigger));
            _serverUrlText.Text = Settings.Default.ServerUrl;
            Initialize();
        }

        private IEnumerable<Job> SelectedJobs
        {
            get { return _jobsGrid.SelectedRows.Cast<DataGridViewRow>().Select(row => (Job) row.DataBoundItem); }
        }

        private IEnumerable<Trigger> SelectedTriggers
        {
            get { return _triggersGrid.SelectedRows.Cast<DataGridViewRow>().Select(row => (Trigger) row.DataBoundItem); }
        }

        private IEnumerable<Execution> SelectedExecutions
        {
            get { return _executionsGrid.SelectedRows.Cast<DataGridViewRow>().Select(row => (Execution) row.DataBoundItem); }
        }

        private void _serverUrlText_Leave(object sender, EventArgs e)
        {
            Settings.Default.ServerUrl = _serverUrlText.Text;
            Initialize();
        }

        private void Initialize()
        {
            _client = new SDataClient(_serverUrlText.Text)
                {
                    Credentials = _credentials,
                    NamingScheme = NamingScheme.CamelCase,
                    Trace =
                        {
                            Switch = {Level = SourceLevels.Information},
                            Listeners = {new TextBoxTraceListener(_logText, _serverUrlText.Text.Length) {Filter = new EventTypeFilter(SourceLevels.All)}}
                        }
                };
            Settings.Default.Save();

            _jobsGrid.AutoGenerateColumns = true;
            _triggersGrid.AutoGenerateColumns = true;
            _executionsGrid.AutoGenerateColumns = true;

            _jobsGrid.DataSource = new Job[0];
            _jobParametersGrid.DataSource = new JobParameter[0];
            _jobStateGrid.DataSource = new State[0];
            _triggersGrid.DataSource = new Trigger[0];
            _triggerParametersGrid.DataSource = new TriggerParameter[0];
            _executionsGrid.DataSource = new Execution[0];
            _executionStateGrid.DataSource = new State[0];

            _jobsGrid.Columns.Remove("Parameters");
            _jobsGrid.Columns.Remove("State");
            _jobsGrid.AutoGenerateColumns = false;
            _triggersGrid.Columns.Remove("Parameters");
            _triggersGrid.AutoGenerateColumns = false;
            _executionsGrid.Columns.Remove("State");
            _executionsGrid.AutoGenerateColumns = false;

            _jobsGrid.AutoResizeColumns();
            _jobParametersGrid.AutoResizeColumns();
            _jobStateGrid.AutoResizeColumns();
            _triggersGrid.AutoResizeColumns();
            _triggerParametersGrid.AutoResizeColumns();
            _executionsGrid.AutoResizeColumns();
            _executionStateGrid.AutoResizeColumns();
        }

        private void _refreshAllJobsButton_Click(object sender, EventArgs e)
        {
            _jobsGrid.DataSource = new BindingList<Job>(_client.Query<Job>().ToList());
            _jobsGrid.AutoResizeColumns();
        }

        private void _refreshJobButton_Click(object sender, EventArgs e)
        {
            foreach (var job in SelectedJobs)
            {
                try
                {
                    var updated = _client.Get<Job>(job.Key);
                    CopyObjectValues(updated, job);
                }
                catch (SDataException ex)
                {
                    MessageBox.Show(string.Format("Error refreshing job '{0}'\r\n{1}", job.Key, ex.Message));

                    if (ex.StatusCode == HttpStatusCode.Gone)
                    {
                        ((ICollection<Job>) _jobsGrid.DataSource).Remove(job);
                    }
                }
            }

            _jobsGrid.Refresh();
            _jobsGrid.AutoResizeColumns();
            _jobParametersGrid.DataSource = _jobsGrid.SelectedRows.Count == 1 ? SelectedJobs.First().Parameters : null;
            _jobParametersGrid.AutoResizeColumns();
            _jobStateGrid.DataSource = _jobsGrid.SelectedRows.Count == 1 ? SelectedJobs.First().State : null;
            _jobStateGrid.AutoResizeColumns();
        }

        private void _triggerJobButton_Click(object sender, EventArgs e)
        {
            foreach (var job in SelectedJobs)
            {
                try
                {
// ReSharper disable once AccessToForEachVariableInClosure
                    MessageBox.Show(_client.CallService(() => job.Trigger()));
                }
                catch (SDataException ex)
                {
                    MessageBox.Show(string.Format("Error triggering job '{0}'\r\n{1}", job.Key, ex.Message));
                }
            }
        }

        private void _interuptJobButton_Click(object sender, EventArgs e)
        {
            foreach (var job in SelectedJobs)
            {
// ReSharper disable once AccessToForEachVariableInClosure
                if (!_client.CallService(() => job.Interrupt()))
                {
                    MessageBox.Show(string.Format("Failed to interrupt job '{0}'", job.Key));
                }
            }
        }

        private void _pauseJobButton_Click(object sender, EventArgs e)
        {
            foreach (var job in SelectedJobs)
            {
// ReSharper disable once AccessToForEachVariableInClosure
                _client.CallService(() => job.Pause());
            }
        }

        private void _resumeJobButton_Click(object sender, EventArgs e)
        {
            foreach (var job in SelectedJobs)
            {
// ReSharper disable once AccessToForEachVariableInClosure
                _client.CallService(() => job.Resume());
            }
        }

        private void _jobsGrid_SelectionChanged(object sender, EventArgs e)
        {
            var selected = _jobsGrid.SelectedRows.Count > 0;
            _jobParametersGrid.DataSource = _jobsGrid.SelectedRows.Count == 1 ? SelectedJobs.First().Parameters : null;
            _jobParametersGrid.AutoResizeColumns();
            _jobStateGrid.DataSource = _jobsGrid.SelectedRows.Count == 1 ? SelectedJobs.First().State : null;
            _jobStateGrid.AutoResizeColumns();
            _refreshJobButton.Enabled = selected;
            _triggerJobButton.Enabled = selected;
            _interuptJobButton.Enabled = selected;
            _pauseJobButton.Enabled = selected;
            _resumeJobButton.Enabled = selected;
            _createTriggerButton.Enabled = _jobsGrid.SelectedRows.Count == 1;
        }

        private void _refreshAllTriggersButton_Click(object sender, EventArgs e)
        {
            _triggersGrid.DataSource = new BindingList<Trigger>(_client.Query<Trigger>().ToList());
            _triggersGrid.AutoResizeColumns();
        }

        private void _refreshTriggerButton_Click(object sender, EventArgs e)
        {
            foreach (var trigger in SelectedTriggers)
            {
                try
                {
                    var updated = _client.Get<Trigger>(trigger.Key);
                    CopyObjectValues(updated, trigger);
                }
                catch (SDataException ex)
                {
                    MessageBox.Show(string.Format("Error refreshing trigger '{0}'\r\n{1}", trigger.Key, ex.Message));

                    if (ex.StatusCode == HttpStatusCode.Gone)
                    {
                        ((ICollection<Trigger>) _triggersGrid.DataSource).Remove(trigger);
                    }
                }
            }

            _triggersGrid.Refresh();
            _triggersGrid.AutoResizeColumns();
            _triggerParametersGrid.DataSource = _triggersGrid.SelectedRows.Count == 1 ? SelectedTriggers.First().Parameters : null;
            _triggerParametersGrid.AutoResizeColumns();
        }

        private void _pauseTriggerButton_Click(object sender, EventArgs e)
        {
            foreach (var trigger in SelectedTriggers)
            {
// ReSharper disable once AccessToForEachVariableInClosure
                _client.CallService(() => trigger.Pause());
                trigger.Status = TriggerStatus.Paused;
            }

            _triggersGrid.Refresh();
            _triggersGrid.AutoResizeColumns();
        }

        private void _resumeTriggerButton_Click(object sender, EventArgs e)
        {
            foreach (var trigger in SelectedTriggers)
            {
// ReSharper disable once AccessToForEachVariableInClosure
                _client.CallService(() => trigger.Resume());
                trigger.Status = TriggerStatus.Normal;
            }

            _triggersGrid.Refresh();
            _triggersGrid.AutoResizeColumns();
        }

        private void _createTriggerButton_Click(object sender, EventArgs e)
        {
            var job = SelectedJobs.First();
            var trigger = new Trigger
                {
                    Job = new SDataResource {Key = job.Key},
                    Parameters = job.Parameters != null ? job.Parameters.Select(param => new TriggerParameter {Name = param.Name, Value = param.DefaultValue}).ToList() : null
                };

            using (var form = new EditForm())
            {
                if (form.ShowDialog(trigger, this) != DialogResult.OK)
                {
                    return;
                }
            }

            Trigger created;
            try
            {
                created = _client.Post(trigger);
            }
            catch (SDataException ex)
            {
                MessageBox.Show(string.Format("Error creating trigger '{0}'\r\n{1}", trigger.Key, ex.Message));
                return;
            }

            var list = _triggersGrid.DataSource as ICollection<Trigger>;
            if (list != null && list.Count > 0)
            {
                list.Add(created);
            }
            else
            {
                _triggersGrid.DataSource = new BindingList<Trigger>(new List<Trigger> {created});
            }

            _triggersGrid.Refresh();
            _triggersGrid.AutoResizeColumns();
        }

        private void _updateTriggerButton_Click(object sender, EventArgs e)
        {
            foreach (var trigger in SelectedTriggers)
            {
                using (var form = new EditForm())
                {
                    if (form.ShowDialog(trigger, this) != DialogResult.OK)
                    {
                        return;
                    }
                }

                try
                {
                    var updated = _client.Put(trigger);
                    CopyObjectValues(updated, trigger);
                }
                catch (SDataException ex)
                {
                    MessageBox.Show(string.Format("Error updating trigger '{0}'\r\n{1}", trigger.Key, ex.Message));

                    if (ex.StatusCode == HttpStatusCode.Gone)
                    {
                        ((ICollection<Trigger>) _triggersGrid.DataSource).Remove(trigger);
                    }
                }
            }

            _triggersGrid.Refresh();
            _triggersGrid.AutoResizeColumns();
        }

        private void _deleteTriggerButton_Click(object sender, EventArgs e)
        {
            foreach (var trigger in SelectedTriggers)
            {
                _client.Delete(trigger);
                ((ICollection<Trigger>) _triggersGrid.DataSource).Remove(trigger);
            }

            _triggersGrid.Refresh();
            _triggersGrid.AutoResizeColumns();
        }

        private void _triggersGrid_SelectionChanged(object sender, EventArgs e)
        {
            var selected = _triggersGrid.SelectedRows.Count > 0;
            _triggerParametersGrid.DataSource = _triggersGrid.SelectedRows.Count == 1 ? SelectedTriggers.First().Parameters : null;
            _triggerParametersGrid.AutoResizeColumns();
            _refreshTriggerButton.Enabled = selected;
            _pauseTriggerButton.Enabled = selected;
            _resumeTriggerButton.Enabled = selected;
            _updateTriggerButton.Enabled = _triggersGrid.SelectedRows.Count == 1;
            _deleteTriggerButton.Enabled = selected;
        }

        private void _refreshAllExecutionsButton_Click(object sender, EventArgs e)
        {
            _executionsGrid.DataSource = new BindingList<Execution>(_client.Query<Execution>().ToList());
            _executionsGrid.AutoResizeColumns();
        }

        private void _refreshExecutionButton_Click(object sender, EventArgs e)
        {
            foreach (var execution in SelectedExecutions)
            {
                try
                {
                    var updated = _client.Get<Execution>(execution.Key);
                    CopyObjectValues(updated, execution);
                }
                catch (SDataException ex)
                {
                    MessageBox.Show(string.Format("Error refreshing execution '{0}'\r\n{1}", execution.Key, ex.Message));

                    if (ex.StatusCode == HttpStatusCode.Gone)
                    {
                        ((ICollection<Execution>) _executionsGrid.DataSource).Remove(execution);
                    }
                }
            }

            _executionsGrid.Refresh();
            _executionsGrid.AutoResizeColumns();
            _executionStateGrid.DataSource = _executionsGrid.SelectedRows.Count == 1 ? SelectedExecutions.First().State : null;
            _executionStateGrid.AutoResizeColumns();
        }

        private void _resultExecutionButton_Click(object sender, EventArgs e)
        {
            var refresh = false;

            foreach (var execution in SelectedExecutions)
            {
                try
                {
                    var results = _client.Execute<string>(new SDataParameters
                        {
                            Path = "executions(" + SDataUri.FormatConstant(execution.Key) + ")/result"
                        });
                    using (var form = new ResultForm())
                    {
                        form.ShowDialog(results.Content, this);
                    }
                }
                catch (SDataException ex)
                {
                    MessageBox.Show(string.Format("Error getting execution '{0}' result\r\n{1}", execution.Key, ex.Message));

                    if (ex.StatusCode == HttpStatusCode.Gone)
                    {
                        ((ICollection<Execution>) _executionsGrid.DataSource).Remove(execution);
                        refresh = true;
                    }
                }
            }

            if (refresh)
            {
                _executionsGrid.Refresh();
                _executionsGrid.AutoResizeColumns();
            }
        }

        private void _interruptExecutionButton_Click(object sender, EventArgs e)
        {
            foreach (var execution in SelectedExecutions)
            {
// ReSharper disable once AccessToForEachVariableInClosure
                if (!_client.CallService(() => execution.Interrupt()))
                {
                    MessageBox.Show(string.Format("Failed to interrupt execution '{0}'", execution.Key));
                }
            }
        }

        private void _deleteExecutionButton_Click(object sender, EventArgs e)
        {
            foreach (var execution in SelectedExecutions)
            {
                _client.Delete(execution);
                ((ICollection<Execution>) _executionsGrid.DataSource).Remove(execution);
            }

            _executionsGrid.Refresh();
            _executionsGrid.AutoResizeColumns();
        }

        private void _executionsGrid_SelectionChanged(object sender, EventArgs e)
        {
            var selected = _executionsGrid.SelectedRows.Count > 0;
            _executionStateGrid.DataSource = _executionsGrid.SelectedRows.Count == 1 ? SelectedExecutions.First().State : null;
            _executionStateGrid.AutoResizeColumns();
            _refreshExecutionButton.Enabled = selected;
            _resultExecutionButton.Enabled = _executionsGrid.SelectedRows.Count == 1;
            _interruptExecutionButton.Enabled = selected;
            _deleteExecutionButton.Enabled = selected;
        }

        private static void CopyObjectValues<T>(T from, T to)
        {
            foreach (var prop in TypeDescriptor.GetProperties(typeof (T)).Cast<PropertyDescriptor>())
            {
                prop.SetValue(to, prop.GetValue(from));
            }
        }

        #region Nested type: TextBoxTraceListener

        private class TextBoxTraceListener : TraceListener
        {
            private readonly TextBox _box;
            private readonly int _urlOffset;
            private int _lastId = 1;

            public TextBoxTraceListener(TextBox box, int urlOffset)
            {
                _box = box;
                _urlOffset = urlOffset;
            }

            public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
            {
                var message = string.Empty;

                if (id == 0)
                {
                    var request = (SDataRequest) data;
                    if (_lastId == 0)
                    {
                        message = Environment.NewLine;
                    }
                    message += string.Format("{0:hh\\:mm\\:ss} {1,6} {2}",
                        DateTime.Now.TimeOfDay,
                        request.Method.ToString().ToUpper(),
                        request.Uri.Substring(_urlOffset));
                }
                else
                {
                    var response = data as SDataResponse;
                    if (response != null)
                    {
                        message = string.Format(" -> ({0}) {1}",
                            (int) response.StatusCode,
                            response.StatusCode);
                    }
                    message += Environment.NewLine;
                }

                _box.Invoke(new Action(() => _box.AppendText(message)));
                _lastId = id;
            }

            public override void Write(string message)
            {
                throw new NotSupportedException();
            }

            public override void WriteLine(string message)
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Nested type: PromptCredentials

        private class PromptCredentials : ICredentials
        {
            private readonly CredentialCache _cache = new CredentialCache();

            public NetworkCredential GetCredential(Uri uri, string authType)
            {
                if (authType != "basic")
                {
                    return null;
                }
                var credential = _cache.GetCredential(uri, authType);
                if (credential == null)
                {
                    using (var prompt = new UserCredentialsDialog
                        {
                            Target = new SDataUri(uri) {Path = null, Query = null}.ToString(),
                            User = Settings.Default.UserName
                        })
                    {
                        if (prompt.ShowDialog() == DialogResult.Cancel)
                        {
                            throw new OperationCanceledException();
                        }
                        credential = new NetworkCredential(prompt.User, prompt.Password);
                        _cache.Add(uri, authType, credential);
                        Settings.Default.UserName = prompt.User;
                    }
                }
                return credential;
            }
        }

        #endregion
    }
}