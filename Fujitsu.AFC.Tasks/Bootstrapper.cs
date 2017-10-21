using AutoMapper;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Tasks
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            ConfigureAutoMapper();
        }

        private static void ConfigureAutoMapper()
        {
#pragma warning disable 618
            //TODO: Background task investigate DI for AutoMapper
            Mapper.CreateMap<Task, HistoryLog>()
                .ForMember(src => src.Id, opt => opt.Ignore())
                .ForMember(src => src.Escalated, opt => opt.Ignore())
                .ForMember(src => src.TaskId, x => x.MapFrom(y => y.Id))
                .ForMember(src => src.EventType, opt => opt.Ignore())
                .ForMember(src => src.EventDetail, opt => opt.Ignore())
                .ForMember(src => src.InsertedBy, opt => opt.Ignore())
                .ForMember(src => src.InsertedDate, opt => opt.Ignore())
                .ForMember(src => src.UpdatedDate, opt => opt.Ignore())
                .ForMember(src => src.UpdatedBy, opt => opt.Ignore());
#pragma warning restore 618
        }

    }
}
