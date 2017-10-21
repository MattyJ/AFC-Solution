using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Fujitsu.AFC.Model;


namespace Fujitsu.AFC.Tasks
{
    public class TaskProfile : Profile
    {
        public override string ProfileName => "TaskMappings";

        protected override void Configure()
        {
            CreateMap<Task, TaskHistoryLog>();
        }
    }
}
