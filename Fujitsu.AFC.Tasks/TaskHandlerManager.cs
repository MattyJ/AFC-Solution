using System;
using Fujitsu.AFC.Tasks.Interfaces;
using Fujitsu.AFC.Tasks.Properties;
using Fujitsu.AFC.Tasks.Resources;

namespace Fujitsu.AFC.Tasks
{
    public abstract class TaskHandlerManager : ITaskHandlerManager
    {
        public abstract void Execute();

        public virtual bool CanExecute(int hour)
        {
            var serviceStartTimeHour = GetConfigurationTime("ServiceStartTimeHour", Settings.Default.ServiceStartTimeHour).Hour;
            var serviceEndTimeHour = GetConfigurationTime("ServiceEndTimeHour", Settings.Default.ServiceEndTimeHour).Hour;

            if (hour < 12 && serviceStartTimeHour > 12 && hour < serviceEndTimeHour)
            {
                return true;
            }

            return hour >= serviceStartTimeHour && hour < serviceEndTimeHour;
        }


        private static DateTime GetConfigurationTime(string settingName, string serviceTimeConfig)
        {
            if (string.IsNullOrEmpty(serviceTimeConfig))
            {
                throw new ArgumentException(string.Format(TaskResources.ConfigurationFile_SettingMissing, settingName));
            }

            var serviceTime = DateTime.MinValue;
            // Convert the time into a time object.
            if (!DateTime.TryParse(serviceTimeConfig, out serviceTime))
            {
                throw new ArgumentException(TaskResources.ConfigurationFile_InvalidRunTime);
            }
            return serviceTime;
        }
    }
}
