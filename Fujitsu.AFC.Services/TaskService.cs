using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.Exceptions.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.AFC.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<Task> _taskRepository;
        private readonly IRepository<HistoryLog> _historyLogRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IRepository<TimerLock> _timerLockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public const string TaskCompletedSuccessfully = "Task Completed Successfully";
        public const string TaskCompleted = "Task Completed";

        public TaskService(IRepository<Task> taskRepository,
            IRepository<TimerLock> timerLockRepository,
            IRepository<HistoryLog> historyLogRepository,
            IUserIdentity userIdentity,
            IUnitOfWork unitOfWork)
        {
            if (taskRepository == null)
            {
                throw new ArgumentNullException(nameof(taskRepository));
            }
            if (timerLockRepository == null)
            {
                throw new ArgumentNullException(nameof(timerLockRepository));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _taskRepository = taskRepository;
            _timerLockRepository = timerLockRepository;
            _historyLogRepository = historyLogRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Task> All()
        {
            IEnumerable<Task> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _taskRepository.All().ToList();
                                      });
            return result;
        }

        public int Create(Task entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _taskRepository.Insert(entity);
                                          _unitOfWork.Save();

                                      });
            return entity.Id;
        }

        public void Update(Task entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _taskRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void UpdateTask(Task task)
        {
            var now = DateTime.Now;

            // Update the task.
            task.UpdatedBy = _userIdentity.Name;
            task.UpdatedDate = now;
            task.CompletedDate = now;
            task.NextScheduledDate = task.NextScheduledDate.HasValue
                ? task.NextScheduledDate.Value.ScheduleNextRunDateTime(task.Frequency)
                : now.ScheduleNextRunDateTime(task.Frequency);

            if (task.Frequency == TaskFrequencyNames.OneTime)
            {
                Delete(task);
            }
            else
            {
                Update(task);
            }
        }

        public void Delete(int id)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                          () =>
                          {
                              _taskRepository.Delete(id);
                              _unitOfWork.Save();
                          });
        }

        public void Delete(Task entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _taskRepository.Delete(entity);
                                          _unitOfWork.Save();

                                      });
        }

        public Task GetById(int id)
        {
            Task result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _taskRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<Task> Query(Expression<Func<Task, bool>> predicate)
        {
            IQueryable<Task> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _taskRepository.Query(predicate).AsNoTracking();
                                      });
            return result;
        }

        public IEnumerable<Task> AllProvisioningHandlerTasks()
        {
            IEnumerable<Task> result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                result = _taskRepository
                    .Query(x => (x.Handler == TaskHandlerNames.ProvisioningHandler) &&
                                (x.Frequency != TaskFrequencyNames.OneTime ||
                                (x.Frequency == TaskFrequencyNames.OneTime && x.CompletedDate.HasValue == false)))
                    .OrderBy(x => x.InsertedDate)
                    .AsNoTracking()
                    .ToList();
            });
            return result;

        }

        public IEnumerable<Task> AllOperationsHandlerTasks()
        {
            IEnumerable<Task> result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                result = _taskRepository
                    .Query(x => (x.Handler == TaskHandlerNames.OperationsHandler) &&
                                (x.Frequency != TaskFrequencyNames.OneTime ||
                                (x.Frequency == TaskFrequencyNames.OneTime && x.CompletedDate.HasValue == false)))
                    .OrderBy(x => x.InsertedDate)
                    .AsNoTracking()
                    .ToList();
            });
            return result;
        }

        public IEnumerable<Task> AllSupportHandlerTasks()
        {
            IEnumerable<Task> result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                result = _taskRepository
                    .Query(x => (x.Handler == TaskHandlerNames.SupportHandler) &&
                                (x.Frequency != TaskFrequencyNames.OneTime ||
                                (x.Frequency == TaskFrequencyNames.OneTime && x.CompletedDate.HasValue == false)))
                    .OrderBy(x => x.InsertedDate)
                .AsNoTracking()
                .ToList();
            });
            return result;
        }

        public void CompleteUnrecoverableTaskException(Task task, string message)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                var timerLock = _timerLockRepository.Find(x => x.TaskId == task.Id).ToList();
                if (timerLock.Any())
                {
                    foreach (var lck in timerLock)
                    {
                        // Explicitly remove the timer lock
                        _timerLockRepository.Delete(lck);
                    }
                }

                // Create a history log
                var historyLog = task.CreateHistoryLog(_userIdentity.Name);
                historyLog.EventType = LoggingEventTypeNames.Error;
                historyLog.EventDetail = message;
                historyLog.Escalated = false;
                _historyLogRepository.Insert(historyLog);

                // Delete the task
                _taskRepository.Delete(task);
                _unitOfWork.Save();
            });
        }

        public void CompleteTask(Task task, bool success)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                // Release the lock
                var timerLock = _timerLockRepository.Find(x => x.TaskId == task.Id).ToList();
                if (timerLock.Any())
                {
                    foreach (var lck in timerLock)
                    {
                        // Explicitly remove the timer lock
                        _timerLockRepository.Delete(lck);
                    }
                }

                // Update the task
                UpdateTask(task);

                // Create the journal entry
                var historyLog = task.CreateHistoryLog(_userIdentity.Name);
                if (success)
                {
                    historyLog.EventType = LoggingEventTypeNames.Information;
                    historyLog.EventDetail = TaskCompletedSuccessfully;
                }
                else
                {
                    historyLog.EventType = LoggingEventTypeNames.Warning;
                    historyLog.EventDetail = TaskCompleted;
                }
                _historyLogRepository.Insert(historyLog);

                // Save the Unit Of Work
                _unitOfWork.Save();
            });
        }

        public bool PendingAllocatePinOperation(int pin, DateTime insertedDate)
        {
            return _taskRepository.Query(x => x.Handler == TaskHandlerNames.OperationsHandler
                && x.Name == TaskNames.AllocatePin
                && x.Pin == pin
                && x.InsertedDate < insertedDate).AsNoTracking().Any();
        }

        public bool PendingMergeFromPinOperation(int pin, DateTime insertedDate)
        {
            return _taskRepository.Query(x => x.Handler == TaskHandlerNames.OperationsHandler
                && x.Name == TaskNames.MergePin
                && x.FromPin.Value == pin
                && x.InsertedDate < insertedDate).AsNoTracking().Any();
        }

        public bool PendingMergeToPinOperation(int pin, DateTime insertedDate)
        {
            return _taskRepository.Query(x => x.Handler == TaskHandlerNames.OperationsHandler
                && x.Name == TaskNames.MergePin
                && x.ToPin.Value == pin
                && x.InsertedDate < insertedDate).AsNoTracking().Any();
        }
    }
}
