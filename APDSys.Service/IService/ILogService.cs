using APDSys.Model.DTO.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.IService
{
    public interface ILogService
    {
        Task SaveNewLog(string userName, string description);
        Task<IEnumerable<GetLogDTO>> GetLogsAsync();
        Task<IEnumerable<GetLogDTO>> GetMyLogsAsync(ClaimsPrincipal user);
    }
}
