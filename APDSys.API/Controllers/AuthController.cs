using APDSys.Model.DTO.Auth;
using APDSys.Service.IService;
using APDSys.Utility.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APDSys.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Seed roles to database
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("seed-role")]
        public async Task<IActionResult> SeedRoles()
        {
            var seedResult = await _authService.SeedRolesAsync();

            return StatusCode(seedResult.StatusCode, seedResult.Message);
        }

        /// <summary>
        /// Register an account
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var registerResult = await _authService.RegisterAsync(registerDTO);
            return StatusCode(registerResult.StatusCode, registerResult.Message);
        }

        /// <summary>
        /// Login into system
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginServiceResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var loginResult = await _authService.LoginAsync(loginDTO);

            if (loginResult is null)
            {
                return Unauthorized("Your credentials are invalid. Please contact to an Admin");
            }

            return Ok(loginResult);
        }

        /// <summary>
        /// Update user role.
        /// Owner can change everything.
        /// Admin can change User and Manager or reverse.
        /// Manager and User don't have access to this route.
        /// </summary>
        /// <param name="updateRoleDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-role")]
        [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDTO updateRoleDTO)
        {
            var updateRoleResult = await _authService.UpdateRoleAsync(User, updateRoleDTO);

            if (updateRoleResult.IsSucceed)
            {
                return Ok(updateRoleResult.Message);
            }
            else
            {
                return StatusCode(updateRoleResult.StatusCode, updateRoleResult.Message);
            }
        }

        /// <summary>
        /// getting data of a user from it's JWT
        /// </summary>
        /// <param name="meDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("me")]
        public async Task<ActionResult<LoginServiceResponseDTO>> Me([FromBody] MeDTO token)
        {
            try
            {
                var me = await _authService.MeAsync(token);

                if (me is not null)
                {
                    return Ok(me);
                }
                else
                {
                    return Unauthorized("Invalid token");
                }
            }
            catch (Exception ex)
            {
                return Unauthorized("Invalid token");
            }
        }

        /// <summary>
        /// List of all users with details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("users")]
        public async Task<ActionResult<IEnumerable<UserInfoResult>>> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get a user by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("user/{userName}")]
        public async Task<ActionResult<UserInfoResult>> GetUserByUserName([FromRoute] string userName)
        {
            var user = await _authService.GetUserByUsernameAsync(userName);

            if (user is not null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound("UserName not found");
            }
        }

        /// <summary>
        /// Get list of userNames for send message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("usernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserNames()
        {
            var userNames = await _authService.GetUserNamesAsync();

            return Ok(userNames);
        }



    }
}
