using ADPSys.DataAccess.Context;
using ADPSys.DataAccess.IRepository;
using ADPSys.DataAccess.Repository;
using APDSys.Model.Domain;
using APDSys.Model.DTO.Log;
using APDSys.Service.IService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.Service
{
    public class LogService : ILogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task SaveNewLog(string userName, string description)
        {
            var log = new Log
            {
                UserName = userName,
                Description = description
            };

            await _unitOfWork.LogRepository.AddAsync(log);
            await _unitOfWork.SaveAsync();

        }

        public async Task<IEnumerable<GetLogDTO>> GetLogsAsync()
        {
            var logs = await _unitOfWork.LogRepository.GetAllAsync();


            var logsDTO = _mapper.Map<List<GetLogDTO>>(logs).OrderByDescending(x => x.CreatedAt);

            return logsDTO;
        }

        public async Task<IEnumerable<GetLogDTO>> GetMyLogsAsync(ClaimsPrincipal user)
        {
            var logs = await _unitOfWork.LogRepository.GetAllAsync(x => x.UserName == user.Identity.Name);

            var logsDTO = _mapper.Map<List<GetLogDTO>>(logs).OrderByDescending(x => x.CreatedAt);

            return logsDTO;
        }
    }
}
