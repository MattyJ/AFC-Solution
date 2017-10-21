using System;
using System.Collections.Generic;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Enumerations;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;

namespace Fujitsu.AFC.Operations.Processor
{
    public class OperationTaskProcessor : IOperationTaskProcessor
    {
        private readonly IObjectBuilder _objectBuilder;

        public OperationTaskProcessor(IObjectBuilder objectBuilder)
        {
            if (objectBuilder == null)
            {
                throw new ArgumentNullException(nameof(objectBuilder));
            }

            _objectBuilder = objectBuilder;
        }

        public void Process(Task task)
        {
            if (!Enum.IsDefined(typeof(OperationType), task.Name))
            {
                throw new ArgumentException("OperationProcessorInvalidOperationType");
            }

            var operation = _objectBuilder.Resolve<IOperationProcessor>(task.Name);
            operation.Execute(task);
        }

    }
}
