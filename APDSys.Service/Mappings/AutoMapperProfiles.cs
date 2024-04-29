using APDSys.Model.Domain;
using APDSys.Model.DTO.Log;
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

        }
    }
}
