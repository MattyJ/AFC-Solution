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
    public class SiteService : IService<Site>
    {
        private readonly IRepository<Site> _siteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SiteService(IRepository<Site> siteRepository,
            IUnitOfWork unitOfWork)
        {
            if (siteRepository == null)
            {
                throw new ArgumentNullException(nameof(siteRepository));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _siteRepository = siteRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Site> All()
        {
            IEnumerable<Site> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _siteRepository.All().ToList();
                                      });
            return result;
        }

        public int Create(Site entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _siteRepository.Insert(entity);
                                          _unitOfWork.Save();

                                      });
            return entity.Id;
        }

        public void Update(Site entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _siteRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Site(Site entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _siteRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(int id)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                          () =>
                          {
                              _siteRepository.Delete(id);
                              _unitOfWork.Save();
                          });
        }

        public void Delete(Site entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _siteRepository.Delete(entity);
                                          _unitOfWork.Save();

                                      });
        }

        public Site GetById(int id)
        {
            Site result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _siteRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<Site> Query(Expression<Func<Site, bool>> predicate)
        {
            IQueryable<Site> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _siteRepository.Query(predicate).AsNoTracking();
                                      });
            return result;
        }

    }
}
