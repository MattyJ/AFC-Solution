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
    public class LibraryService : ILibraryService
    {
        private readonly ITaskService _taskService;
        private readonly IRepository<Library> _libraryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LibraryService(ITaskService taskService,
            IRepository<Library> libraryRepository,
            IUnitOfWork unitOfWork)
        {
            if (taskService == null)
            {
                throw new ArgumentNullException(nameof(taskService));
            }
            if (libraryRepository == null)
            {
                throw new ArgumentNullException(nameof(libraryRepository));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _taskService = taskService;
            _libraryRepository = libraryRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Library> All()
        {
            IEnumerable<Library> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _libraryRepository.All().ToList();
                                      });
            return result;
        }

        public int Create(Library entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _libraryRepository.Insert(entity);
                                          _unitOfWork.Save();

                                      });
            return entity.Id;
        }

        public void Update(Library entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _libraryRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Library(Library entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _libraryRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(int id)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                          () =>
                          {
                              _libraryRepository.Delete(id);
                              _unitOfWork.Save();
                          });
        }

        public void Delete(Library entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _libraryRepository.Delete(entity);
                                          _unitOfWork.Save();

                                      });
        }

        public Library GetById(int id)
        {
            Library result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _libraryRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<Library> Query(Expression<Func<Library, bool>> predicate)
        {
            IQueryable<Library> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _libraryRepository.Query(predicate).AsNoTracking();
                                      });
            return result;
        }

        public Dictionary<string, List<Library>> GetSiteCollectionLibraryDictionary(TaskEntity task)
        {
            var result = new Dictionary<string, List<Library>>();

            foreach (var library in _libraryRepository.Query(x => x.ProjectId == task.ProjectId.Value).ToList())
            {
                if (_taskService.PendingMergeFromPinOperation(library.Site.Pin, task.InsertedDate)) continue;

                var key = library.Site.ProvisionedSite.ProvisionedSiteCollection.Name;
                if (!result.ContainsKey(library.Site.ProvisionedSite.ProvisionedSiteCollection.Name))
                {
                    result.Add(key, new List<Library> { library });
                }
                else
                {
                    result[key].Add(library);
                }
            }

            return result;
        }

    }
}
