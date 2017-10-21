using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Services
{
    public class HistoryLogService : IService<HistoryLog>
    {
        private readonly IRepository<HistoryLog> _taskHistoryLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HistoryLogService(IRepository<HistoryLog> taskHistoryLogRepository, IUnitOfWork unitOfWork)
        {
            if (taskHistoryLogRepository == null)
            {
                throw new ArgumentNullException(nameof(taskHistoryLogRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _taskHistoryLogRepository = taskHistoryLogRepository;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<HistoryLog> All()
        {
            IEnumerable<HistoryLog> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _taskHistoryLogRepository.All().ToList();
                                      });
            return result;
        }


        public int Create(HistoryLog entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _taskHistoryLogRepository.Insert(entity);
                                          _unitOfWork.Save();
                                      });
            return entity.Id;
        }

        public void Update(HistoryLog entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _taskHistoryLogRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(int id)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                          () =>
                          {
                              _taskHistoryLogRepository.Delete(id);
                              _unitOfWork.Save();

                          });
        }

        public void Delete(HistoryLog entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _taskHistoryLogRepository.Delete(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public HistoryLog GetById(int id)
        {
            HistoryLog result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _taskHistoryLogRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<HistoryLog> Query(Expression<Func<HistoryLog, bool>> predicate)
        {
            IQueryable<HistoryLog> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _taskHistoryLogRepository.Query(predicate).AsNoTracking();
                                      });
            return result;
        }
    }
}
