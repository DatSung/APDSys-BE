using APDSys.Model.DTO.General;
using APDSys.Model.DTO.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.IService
{
    public interface IMessageService
    {
        Task<GeneralServiceResponseDTO> CreateNeMessageAysnc(ClaimsPrincipal user, CreateMessageDTO createMessageDTO);
        Task<IEnumerable<GetMessageDTO>> GetMessagesAsync();
        Task<IEnumerable<GetMessageDTO>> GetMyMessagesAsync(ClaimsPrincipal user);
    }
}
