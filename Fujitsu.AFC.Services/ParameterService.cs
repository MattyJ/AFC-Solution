using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Data.Interfaces;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Services.Interfaces;
using Fujitsu.Exceptions.Framework;

namespace Fujitsu.AFC.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IRepository<Parameter> _parameterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentity _userIdentity;
        private readonly ICacheManager _cacheManager;

        public ParameterService(IRepository<Parameter> parameterRepository,
            IUnitOfWork unitOfWork,
            IUserIdentity userIdentity,
            ICacheManager cacheManager)
        {
            if (parameterRepository == null)
            {
                throw new ArgumentNullException(nameof(parameterRepository));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }
            if (cacheManager == null)
            {
                throw new ArgumentNullException(nameof(cacheManager));
            }

            _parameterRepository = parameterRepository;
            _unitOfWork = unitOfWork;
            _userIdentity = userIdentity;
            _cacheManager = cacheManager;
        }

        public IEnumerable<Parameter> All()
        {
            IEnumerable<Parameter> result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General, () => { result = _parameterRepository.All().ToList(); });
            return result;
        }

        public int Create(Parameter entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _parameterRepository.Insert(entity);
                _unitOfWork.Save();
            });

            return entity.Id;
        }

        public void Update(Parameter entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Invalidate the cache.
                _cacheManager.Remove(entity.Name);

                _parameterRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(int id)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _parameterRepository.Delete(_parameterRepository.GetById(id));
                _unitOfWork.Save();
            });
        }

        public void Delete(Parameter entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _parameterRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public Parameter GetById(int id)
        {
            Parameter result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _parameterRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<Parameter> Query(Expression<Func<Parameter, bool>> predicate)
        {
            IQueryable<Parameter> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _parameterRepository.Query(predicate).AsNoTracking();
                                      });
            return result;
        }

        public Parameter Find(string parameterName)
        {
            Parameter result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General, () => { result = _parameterRepository.SingleOrDefault(x => x.Name == parameterName); });
            return result;
        }

        public T GetParameterByName<T>(string parameterName)
        {
            var result = default(T);
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                var paramValue = _parameterRepository
                    .Query(x => x.Name == parameterName)
                    .Select(s => s.Value)
                    .SingleOrDefault();
                if (!string.IsNullOrEmpty(paramValue))
                {
                    result = paramValue.ConvertStringToGenericValue<T>();
                }
            });
            return result;
        }

        public T GetParameterByNameAndCache<T>(string parameterName)
        {
            return _cacheManager.ExecuteAndCache(parameterName,
                () => GetParameterByName<T>(parameterName));
        }


        public T GetParameterByNameOrCreate<T>(string parameterName, T defaultValue)
        {
            var result = default(T);

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                var paramValue = _parameterRepository
                    .Query(x => x.Name == parameterName)
                    .Select(s => s.Value)
                    .SingleOrDefault();

                if (string.IsNullOrEmpty(paramValue))
                {
                    var entity = new Parameter
                    {
                        Name = parameterName,
                        Value = defaultValue.ConvertGenericValueToString(),
                        InsertedBy = _userIdentity.Name,
                        InsertedDate = DateTime.Now,
                        UpdatedBy = _userIdentity.Name,
                        UpdatedDate = DateTime.Now
                    };
                    _parameterRepository.Insert(entity);
                    result = defaultValue;
                }
                else
                {
                    result = paramValue.ConvertStringToGenericValue<T>();
                }
            });

            return result;
        }

        public void SaveParameter<T>(string parameterName, T value)
        {
            var dateTime = DateTime.Now;
            var valueString = value.ConvertGenericValueToString();

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Invalidate the cache.
                _cacheManager.Remove(parameterName);

                var entity = _parameterRepository.SingleOrDefault(d => d.Name == parameterName);

                if (entity != null)
                {
                    entity.Value = valueString;
                    entity.UpdatedDate = dateTime;
                    entity.UpdatedBy = _userIdentity.Name;
                    _parameterRepository.Update(entity);
                }
                else
                {
                    entity = new Parameter
                    {
                        Name = parameterName,
                        Value = valueString,
                        InsertedBy = _userIdentity.Name,
                        InsertedDate = dateTime,
                        UpdatedBy = _userIdentity.Name,
                        UpdatedDate = dateTime
                    };

                    _parameterRepository.Insert(entity);
                }

                _unitOfWork.Save();
            });
        }
    }
}
