using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UpdatePinWithDictionaryValuesTests
    {
        private Mock<IPinService> _mockPinService;
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
            _mockTaskService = new Mock<ITaskService>();
            _mockSiteService = new Mock<IService<Site>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _sites = new List<Site>
            {
                new Site
                {
                    Id = 1,
                    Pin = 12345,
                    Title = "Test"
                }
            };

            _mockSiteRepository = MockRepositoryHelper.Create(_sites, (entity, id) => entity.Id == (int)id);
            _siteService = new SiteService(_mockSiteRepository.Object, _mockUnitOfWork.Object);

            _operationTaskProcessor = new UpdatePinWithDictionaryValues(_mockPinService.Object, _mockTaskService.Object, _siteService);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdatePinWithDictionaryValues_Constructor_NoPinService()
        {
            #region Arrange

            #endregion

            #region Act

            new UpdatePinWithDictionaryValues(
                null,
                _mockTaskService.Object,
                _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdatePinWithDictionaryValues_Constructor_NoTaskService()
        {
            #region Arrange

            #endregion

            #region Act

            new UpdatePinWithDictionaryValues(
                _mockPinService.Object,
                null,
                 _mockSiteService.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdatePinWithDictionaryValues_Constructor_NoSiteService()
        {
            #region Arrange

            #endregion

            #region Act

            new UpdatePinWithDictionaryValues(
                _mockPinService.Object,
                _mockTaskService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void UpdatePinWithDictionaryValues_Execute_ValidTaskExecutesTask()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12345,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);

            _mockPinService.Verify(v => v.UpdatePinWithDictionaryValues(It.IsAny<Task>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UpdatePinWithDictionaryValues_Execute_TaskThrowsException_ExceptionIsPropogated()
        {
            _mockPinService.Setup(s => s.UpdatePinWithDictionaryValues(It.IsAny<Task>())).Throws(new Exception());

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12345,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }


        [TestMethod]
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void UpdatePinWithDictionaryValues_Execute_NoPinThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Dictionary = ValidDictionaryXml
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdatePinWithDictionaryValues_Execute_NoPinThrowsExceptionWithInvalidRequestNoPinExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
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
        public void UpdatePinWithDictionaryValues_Execute_NoPinThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
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
        public void UpdatePinWithDictionaryValues_Execute_NoDictionaryXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12346

            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdatePinWithDictionaryValues_Execute_NoDictionaryXmlThrowsExceptionWithInvalidRequestNoDictionaryXmlExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12346

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
        public void UpdatePinWithDictionaryValues_Execute_NoDictionaryXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12346
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
        public void UpdatePinWithDictionaryValues_Execute_InvalidSiteDictionaryXmlThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12345,
                CaseId = 12345,
                Dictionary = "<tests><test>hello</problem></tests>"
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdatePinWithDictionaryValues_Execute_InvalidSiteDictionaryXmlThrowsExceptionWithInvalidRequesInvalidDictionaryXMLExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12345,
                CaseId = 12345,
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
        public void UpdatePinWithDictionaryValues_Execute_InvalidSiteDictionaryXmlThrowsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12345,
                CaseId = 12345,
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
        [ExpectedException(typeof(UnRecoverableErrorException))]
        public void UpdatePinWithDictionaryValues_Execute_PinDoesNotExistsThrowsUnRecoverableErrorException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12346
            };

            _operationTaskProcessor.Execute(task);
        }

        [TestMethod]
        public void UpdatePinWithDictionaryValues_Execute_PinDoesNotExceptionWithInvalidRequestPinDoesNotExistExceptionMessage()
        {
            var message = string.Empty;

            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12346,
                Dictionary = ValidDictionaryXml
            };

            _mockSiteService.Setup(s => s.Query(x => x.Pin == task.Pin.Value)).Returns(_sites.AsQueryable());

            var expectedMessage = string.Format(TaskResources.OperationsTaskRequest_PinDoesNotExist, task.Name, task.Pin.Value);

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
        public void UpdatePinWithDictionaryValues_Execute_PinDoesNotExistsExceptionAndCallsCompleteUnrecoverableTaskException()
        {
            var task = new Task
            {
                Id = 1,
                Frequency = TaskFrequencyNames.OneTime,
                Handler = TaskHandlerNames.OperationsHandler,
                Name = TaskNames.UpdatePinWithDictionaryValues,
                Pin = 12349,
                Dictionary = ValidDictionaryXml
            };

            _mockSiteService.Setup(s => s.Query(x => x.Pin == task.Pin.Value)).Returns(_sites.AsQueryable());

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
