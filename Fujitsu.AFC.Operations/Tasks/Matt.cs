using System;
using Fujitsu.AFC.Model;
using Fujitsu.AFC.Operations.Interfaces;

namespace Fujitsu.AFC.Operations.Tasks
{
    public class Matt : IOperationsTaskProcessor
    {

        public void Execute(Task task)
        {
            Console.WriteLine($"Pin [{task.Pin.Value}] - [{task.Name}]");

            //throw new NotImplementedException();
        }
    }
}
