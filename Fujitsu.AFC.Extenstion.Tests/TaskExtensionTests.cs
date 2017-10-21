using System;
using System.Diagnostics.CodeAnalysis;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Extenstion.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TaskExtensionTests
    {
        private const string LastUserName = "matt.jordan@uk.fujitsu.com";
        private const string UserName = "matthew.jordan@uk.fujitsu.com";

        [TestInitialize]
        public void TestInitialize()
        {
            Bootstrapper.Initialise();
        }


        [TestMethod]
        public void TaskExtension_CreateHistoryLog_UpdatesInsertedAndUpdatedByColumnsAndCompletedDate()
        {
            var now = DateTime.Now;
            var task = new Task
            {
                Id = 1,
                CompletedDate = DateTime.Now.AddDays(-7),
                InsertedDate = DateTime.Now.AddDays(-14),
                InsertedBy = LastUserName,
                UpdatedDate = DateTime.Now.AddDays(-7),
                UpdatedBy = LastUserName
            };

            var historyLog = task.CreateHistoryLog(UserName);

            Assert.AreEqual(now.Year, historyLog.CompletedDate.Year);
            Assert.AreEqual(now.Month, historyLog.CompletedDate.Month);
            Assert.AreEqual(now.Day, historyLog.CompletedDate.Day);
            Assert.AreEqual(now.Year, historyLog.InsertedDate.Year);
            Assert.AreEqual(now.Month, historyLog.InsertedDate.Month);
            Assert.AreEqual(now.Day, historyLog.InsertedDate.Day);
            Assert.AreEqual(now.Year, historyLog.UpdatedDate.Year);
            Assert.AreEqual(now.Month, historyLog.UpdatedDate.Month);
            Assert.AreEqual(now.Day, historyLog.UpdatedDate.Day);
            Assert.AreEqual(UserName, historyLog.InsertedBy);
            Assert.AreEqual(UserName, historyLog.UpdatedBy);
        }

        [TestMethod]
        public void TaskExtension_CreateHistoryLog_SetsTaskId()
        {
            var task = new Task
            {
                Id = 1,
                CompletedDate = DateTime.Now.AddDays(-7),
                InsertedDate = DateTime.Now.AddDays(-14),
                InsertedBy = LastUserName,
                UpdatedDate = DateTime.Now.AddDays(-7),
                UpdatedBy = LastUserName,
            };

            var historyLog = task.CreateHistoryLog(UserName);

            Assert.AreEqual(task.Id, historyLog.TaskId);
        }

    }

}
