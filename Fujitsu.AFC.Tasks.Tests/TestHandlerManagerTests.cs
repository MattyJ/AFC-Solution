using Fujitsu.AFC.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Handlers.Tests
{
    [TestClass]
    public class TestHandlerManagerTests
    {

        private TestTaskHandlerManger _taskHandlerManager;


        [TestInitialize]
        public void TestInitialize()
        {

            _taskHandlerManager = new TestTaskHandlerManger();
        }

        [TestMethod]
        public void TaskHandler_CanExecute_StartTimeNineAmEndTimeTenAmCurrentHourIsStartHour_ReturnsTrue()
        {
            var currentHour = 9;
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceStartTimeHour"] = "09:00:00";
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceEndTimeHour"] = "10:00:00";

            Assert.IsTrue(_taskHandlerManager.CanExecute(currentHour));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_StartTimeNineAmEndTimeTenAmCurrentHourIsEndHour_ReturnsFalse()
        {
            var currentHour = 10;
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceStartTimeHour"] = "09:00:00";
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceEndTimeHour"] = "10:00:00";

            Assert.IsFalse(_taskHandlerManager.CanExecute(currentHour));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_StartTimeMidnightEndTimeSixAmCurrentHourIsOneAm_ReturnsTrue()
        {
            var currentHour = 1;
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceStartTimeHour"] = "00:00:00";
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceEndTimeHour"] = "06:00:00";

            Assert.IsTrue(_taskHandlerManager.CanExecute(currentHour));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_StartTimeNinePmEndTimeSixAmCurrentHourIsOneAm_ReturnsTrue()
        {
            var currentHour = 1;
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceStartTimeHour"] = "21:00:00";
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceEndTimeHour"] = "06:00:00";

            Assert.IsTrue(_taskHandlerManager.CanExecute(currentHour));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_StartTimeNinePmEndTimeSixAmCurrentHourIsEndHour_ReturnsFalse()
        {
            var currentHour = 6;
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceStartTimeHour"] = "21:00:00";
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceEndTimeHour"] = "06:00:00";

            Assert.IsFalse(_taskHandlerManager.CanExecute(currentHour));
        }

        [TestMethod]
        public void TaskHandler_CanExecute_StartTimeNinePmEndTimeSixAmCurrentHourIsEightPm_ReturnsFalse()
        {
            var currentHour = 20;
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceStartTimeHour"] = "21:00:00";
            Fujitsu.AFC.Tasks.Properties.Settings.Default["ServiceEndTimeHour"] = "06:00:00";

            Assert.IsFalse(_taskHandlerManager.CanExecute(currentHour));
        }

        internal class TestTaskHandlerManger : TaskHandlerManager
        {

            public override void Execute()
            {
            }
        }
    }
}
