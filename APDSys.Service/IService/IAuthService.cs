using APDSys.Model.DTO.Auth;
using APDSys.Model.DTO.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.IService
{
    public interface IAuthService
    {
        Task<GeneralServiceResponseDTO> SeedRolesAsync();
        Task<GeneralServiceResponseDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<LoginServiceResponseDTO?> LoginAsync(LoginDTO loginDTO);
        Task<GeneralServiceResponseDTO> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDTO updateRoleDTO);
        Task<LoginServiceResponseDTO?> MeAsync(MeDTO meDTO);
        Task<IEnumerable<UserInfoResult>> GetUsersAsync();
        Task<UserInfoResult?> GetUserByUsernameAsync(string userName);
        Task<IEnumerable<string>> GetUserNamesAsync();
    }
}
