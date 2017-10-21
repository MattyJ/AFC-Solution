using System;
using System.Collections.Generic;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Operations.Tasks;
using Fujitsu.AFC.Services;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.AFC.Tasks.Resources;
using Fujitsu.AFC.UnitTesting;
using Fujitsu.Exceptions.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.AFC.Operations.Tests.Tasks
{
    [TestClass]
    public class AllocatePinTests
    {
        private Mock<IPinService> _mockPinService;
        private Mock<IProvisioningService> _mockProvisioningService;
        private Mock<ITaskService> _mockTaskService;
        private Mock<IService<Site>> _mockSiteService;

        private IService<Site> _siteService;
        private Mock<IRepository<Site>> _mockSiteRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Site> _sites;

        private IOperationsTaskProcessor _operationTaskProcessor;

        private const string ValidDictionaryXml = "<items><item><key>Service Type</key><value>Early Years</value></item><item><key>Service User Pin</key><value>987876678</value></item></items>";

        [TestInitialize]
        public void TestInitialize()
        {

            _mockPinService = new Mock<IPinService>();
            _mockProvisioningService = new Mock<IProvisioningService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockSiteService = new Mock<IService<Site>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Pin = 12346,
                    Title = "Test",
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new AllocatePin(_mockProvisioningService.Object, _mockPinService.Object, _mockTaskService.Object, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllocatePin_Constructor_NoProvisioningService()
        {
            #region Arrange

            #endregion

            #region Act

            new AllocatePin(
                null,
                _mockPinService.Object,
                _mockTaskService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllocatePin_Constructor_NoPinService()
        {
            #region Arrange

            #endregion

            #region Act

            new AllocatePin(
                _mockProvisioningService.Object,
                null,
                _mockTaskService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllocatePin_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new AllocatePin(
                _mockProvisioningService.Object,
                _mockPinService.Object,
                null,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllocatePin_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new AllocatePin(
                _mockProvisioningService.Object,
                _mockPinService.Object,
                _mockTaskService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void AllocatePin_Execute_ValidTaskWithUnallocatedSitesAvailablePassesPreLimChecksAndExecutesTask()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(10);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site",
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);

            _mockPinService.Verify(v => v.AllocatePin(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AllocatePin_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(10);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);
            _mockPinService.Setup(s => s.AllocatePin(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site",
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);

            _mockPinService.Verify(v => v.AllocatePin(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void AllocatePin_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoPin, task.Name);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void AllocatePin_Execute_NoSiteTitleThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoSiteTitleThrowsExceptionWithInvalidRequestNoSiteTitleExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoSiteTitle, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoSiteTitleThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void AllocatePin_Execute_NoDictionaryXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site"

            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoDictionaryXmlThrowsExceptionWithInvalidRequestNoDictionaryXmlExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site"

            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidRequestNoDictionaryXML, task.Name);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoDictionaryXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site"
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void AllocatePin_Execute_InvalidSiteDictionaryXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = "<tests><test>hello</problem></tests>"
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_InvalidSiteDictionaryXmlThrowsExceptionWithInvalidRequestInvalidDictionaryXMLExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = "<tests><test>hello</problem></tests>"
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_InvalidDictionaryXML, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_InvalidSiteDictionaryXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = "<items><item>hello</item></items>"
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AllocatePin_Execute_NoUnallocatedSitesThrowsException()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(0);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site",
                Dictionary = ValidDictionaryXml

            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoUnallocatedSitesThrowsExceptionWithNoUnallocatedSitesAvailableExceptionMessage()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(0);
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site",
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_NoUnallocatedSitesAvailable, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (ApplicationException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_NoUnallocatedSitesThrowsExceptionWithNoUnallocatedSitesCallsAndDoesNotCallCompleteUnrecoverableTaskException()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(0);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "Example PIN Site",
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (ApplicationException)
            {
            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Never);
        }
        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void AllocatePin_Execute_PinAlreadyExistsThrowsUnRecoverableErrorException()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(1);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(false);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12346,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_PinAlreadyExistsExceptionWithInvalidRequestPinAlreadyExistsExceptionMessage()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(1);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12346,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_PinAlreadyExists, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_PinAlreadyExistsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(1);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void AllocatePin_Execute_PendingAllocatePinOperationThrowsUnRecoverableErrorException()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(1);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void AllocatePin_Execute_PendingAllocatePinOperationExceptionWithInvalidRequestPinRequestPendingExceptionMessage()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(1);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_PinRequestPending, task.Name, task.Pin.Value);

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void AllocatePin_Execute_PendingAllocatePinOperationExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            _mockProvisioningService.Setup(s => s.GetNumberOfUnallocatedSites()).Returns(1);
            _mockTaskService.Setup(s => s.PendingAllocatePinOperation(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(true);

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.AllocatePin,
                Pin = 12345,
                SiteTitle = "An Example Site",
                Dictionary = ValidDictionaryXml
            };

            try
            {
                _operationTaskProcessor.Execute(task);
            }
            catch (UnRecoverableErrorException)
            {

            }

            _mockTaskService.Verify(v => v.CompleteUnrecoverableTaskException(It.IsAny<Task>(), It.IsAny<string>()), Times.Once);
        }
    }
}
