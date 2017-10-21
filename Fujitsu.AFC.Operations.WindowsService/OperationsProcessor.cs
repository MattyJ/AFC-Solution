using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using System.Timers;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Operations.WindowsService.Interface;
using Fujitsu.AFC.Operations.WindowsService.Properties;
using Fujitsu.AFC.Tasks.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.Exceptions.Framework;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Fujitsu.AFC.Operations.WindowsService
{
    [ExcludeFromCodeCoverage]
    partial class OperationsProcessor : ServiceBase, IOperationsProcessor
    {
        private readonly IObjectBuilder _objectBuilder;
        private double _interval;
        private Timer _timer;

        public OperationsProcessor(IObjectBuilder objectBuilder)
        {

            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }

            _objectBuilder = objectBuilder;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Initialise();
            LogSettings();
            _timer = new Timer
            {
                Interval = _interval,
                AutoReset = true
            };
            _timer.Elapsed += Execute;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();

            // Free system resources used by the Timer object
            _timer.Dispose();
            _timer = null;
        }

        private void Execute(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Stop();

            try
            {
                var taskHandler = _objectBuilder.Resolve<ITaskHandlerManager>(TaskHandlerNames.OperationsHandler);
                if (taskHandler.CanExecute(DateTime.Now.Hour))
                {
                    taskHandler.Execute();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Flatten(), EventLogEntryType.Error);
            }

            _timer.Start();
        }

        private void Initialise()
        {
            // Set the LogWriter for the static Logger class
            Logger.SetLogWriter(new LogWriterFactory().Create());

            // Initialise the Exception Manager
            var config = ConfigurationSourceFactory.Create();
            var factory = new ExceptionPolicyFactory(config);

            // Set the Exception Policy
            var exceptionManager = factory.CreateManager();
            ExceptionPolicy.SetExceptionManager(exceptionManager);

            _interval = GetPollingInterval();
        }

        private double GetPollingInterval()
        {
            var interval = Settings.Default.PollingIntervalMinutes;
            if (interval <= 0)
            {
                throw new ArgumentException(string.Format(TaskResources.ConfigurationFile_SettingMissing, "PollingIntervalMinutes"));
            }

            return Convert.ToDouble(interval * 60000);
        }

        private void LogSettings()
        {
            EventLog.WriteEntry($"Operations Task Processor Service Instance Id: {Fujitsu.AFC.Tasks.Properties.Settings.Default.ServiceInstanceId}", EventLogEntryType.Information);
            EventLog.WriteEntry($"Operations Task Processor Start Time: {Fujitsu.AFC.Tasks.Properties.Settings.Default.ServiceStartTimeHour}", EventLogEntryType.Information);
            EventLog.WriteEntry($"Operations Task Processor End Time: {Fujitsu.AFC.Tasks.Properties.Settings.Default.ServiceEndTimeHour}", EventLogEntryType.Information);
            EventLog.WriteEntry($"Operations Task Processor Polling Interval: {Settings.Default.PollingIntervalMinutes} minutes", EventLogEntryType.Information);
        }
    }
}
