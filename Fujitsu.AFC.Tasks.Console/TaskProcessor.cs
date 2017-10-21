using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Core.Injection;
using Fujitsu.AFC.Core.Interfaces;
using Fujitsu.AFC.Core.Metrics;
using Fujitsu.AFC.Extensions;
using Fujitsu.AFC.Tasks.Interfaces;

namespace Fujitsu.AFC.Tasks.Console
{
    [ExcludeFromCodeCoverage]
    public class TaskProcessor
    {
        private static readonly List<string> ValidParameters = new List<string>
        {
            TaskHandlerNames.ProvisioningHandler,
            TaskHandlerNames.OperationsHandler,
            TaskHandlerNames.SupportHandler,
            "All"
        };

        private static readonly List<string> Commands = new List<string>
        {
            TaskHandlerNames.ProvisioningHandler,
            TaskHandlerNames.OperationsHandler,
            TaskHandlerNames.SupportHandler
        };

        static void Main(string[] args)
        {
            var objectBuilder = new ObjectBuilder(UnityConfig.RegisterTypes);
            LoggingConfig.Initialise(objectBuilder.GetContainer());
            Bootstrapper.Initialise();

            if (args.Length != 1)
            {
                Usage();
                return;
            }
            var command = args[0];
            if (!ValidParameters.Contains(command))
            {
                Usage();
                return;
            }

            if (command.SafeEquals("All"))
            {
                foreach (var cmd in Commands)
                {
                    ExecuteCommand(cmd, objectBuilder);
                }
            }
            else
            {
                ExecuteCommand(command, objectBuilder);
            }

            System.Console.ReadLine();
        }

        private static void ExecuteCommand(string command, IObjectBuilder objectBuilder)
        {
            try
            {
                var prfMon = new PrfMon();
                System.Console.WriteLine($"Fujitsu.AFC.TaskProcessor.Console -> {command}.");
                var taskHandler = objectBuilder.Resolve<ITaskHandlerManager>(command);
                if (taskHandler.CanExecute(DateTime.Now.Hour))
                {
                    taskHandler.Execute();
                }
                System.Console.WriteLine("Fujitsu.AFC.TaskProcessor.Console -> {0} Completed Processing. Duration: {1:0.000}s", command, prfMon.Stop());
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An exception has occurred. See below for further details.");
                System.Console.WriteLine("    Message: {0}", ex.Message);
                System.Console.WriteLine("    Stack Trace: {0}", ex.StackTrace);
            }
        }

        private static void Usage()
        {
            System.Console.WriteLine("Usage: Fujitsu.AFC.TaskProcessor.Console.exe [task handler manager]");
            System.Console.WriteLine("    where [task handler] is one of:");
            System.Console.WriteLine("        ProvisioningHandler");
            System.Console.WriteLine("        OperationsHandler");
            System.Console.WriteLine("        SupportHandler");
        }



    }
}
