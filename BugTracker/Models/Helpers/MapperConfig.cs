using AutoMapper;
using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.Helpers
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new TicketProfile());
            });
        }
    }

    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<IEnumerable<Project>, IEnumerable<SelectListItem>>();
        }
    }
}