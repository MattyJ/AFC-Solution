using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Services
{
    public class TimerLockService : ITimerLockService
    {
        private readonly IRepository<TimerLock> _timerLockRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public TimerLockService(IRepository<TimerLock> timerLockRepository, IUserIdentity userIdentity, IUnitOfWork unitOfWork)
        {
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

            _timerLockRepository = timerLockRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TimerLock> All()
        {
            IEnumerable<TimerLock> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _timerLockRepository.All().ToList();
                                      });
            return result;
        }

        public int Create(TimerLock entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _timerLockRepository.Insert(entity);
                                          _unitOfWork.Save();
                                      });
            return entity.Id;
        }

        public void Update(TimerLock entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _timerLockRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(int id)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _timerLockRepository.Delete(id);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(TimerLock entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _timerLockRepository.Delete(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public TimerLock GetById(int id)
        {
            TimerLock result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _timerLockRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<TimerLock> Query(Expression<Func<TimerLock, bool>> predicate)
        {
            IQueryable<TimerLock> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _timerLockRepository.Query(predicate).AsNoTracking();
                                      });
            return result;
        }

        public void AcquireLock(Guid serviceInstanceId, int pinId, int taskId)
        {
            if (pinId == 0) return;

            var dateTime = DateTime.Now;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    // Check whether lock already exists
                    var timerLock = _timerLockRepository.FirstOrDefault(x => x.LockedInstance == serviceInstanceId && x.LockedPin == pinId);

                    if (timerLock == null)
                    {
                        // Create new lock
                        var newLock = new TimerLock
                        {
                            LockedInstance = serviceInstanceId,
                            LockedPin = pinId,
                            TaskId = taskId,
                            InsertedBy = _userIdentity.Name,
                            InsertedDate = dateTime,
                            UpdatedBy = _userIdentity.Name,
                            UpdatedDate = dateTime
                        };

                        _timerLockRepository.Insert(newLock);
                        _unitOfWork.Save();

                    }

                });
        }
    }
}
