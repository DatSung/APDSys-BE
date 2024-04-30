using APDSys.Model.Domain;
using APDSys.Model.DTO.Auth;
using APDSys.Model.DTO.Log;
using APDSys.Model.DTO.Message;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Log
            CreateMap<Log, GetLogDTO>().ReverseMap();

            // Message
            CreateMap<Message, GetMessageDTO>().ReverseMap();

            // ApplicationUser
            CreateMap<RegisterDTO, ApplicationUser>()
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(x => Guid.NewGuid().ToString()))
                .ReverseMap();
        }
    }
}
