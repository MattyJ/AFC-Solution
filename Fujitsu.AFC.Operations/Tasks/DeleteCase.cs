using System;
using System.Diagnostics;
using System.Linq;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;
using Fujitsu.AFC.Services.Interfaces;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class DeleteCase : IOperationsTaskProcessor
    {
        private readonly ICaseService _caseService;
        private readonly ILibraryService _libraryService;
        private readonly IParameterService _parameterService;

        public DeleteCase(ICaseService caseService,
            ILibraryService libraryService,
            IParameterService parameterService)
        {
            if (caseService == null)
            {
                throw new ArgumentNullException(nameof(caseService));
            }
            if (libraryService == null)
            {
                throw new ArgumentNullException(nameof(libraryService));
            }
            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            _caseService = caseService;
            _libraryService = libraryService;
            _parameterService = parameterService;
        }

        public void Execute(Task task)
        {
            Debug.WriteLine("Fujitsu.AFC.Operations.DeleteCase.cs -> Processing Started.");
            var prfMonMethod = new PrfMon();

            var retentionPeriodInDays = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.DigitialCaseFileRetentionPeriodInWeeks) * 7;
            foreach (var library in _libraryService.Query(x => x.IsClosed).ToList().Where(y => DateTime.Now > y.UpdatedDate.AddDays(retentionPeriodInDays)))
            {
                task.CaseId = library.CaseId;
                _caseService.DeleteCase(task);
            }

            Debug.WriteLine("Fujitsu.AFC.Operations.DeleteCase.cs -> Completed Processing - Duration: {0:0.000}s", prfMonMethod.Stop());
        }
    }
}
