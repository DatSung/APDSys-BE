using APDSys.Model.Domain;
using APDSys.Model.DTO.Auth;
using APDSys.Model.DTO.General;
using APDSys.Service.IService;
using APDSys.Utility.Constants;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogService logService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logService = logService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<GeneralServiceResponseDTO> SeedRolesAsync()
        {
            bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isManagerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.MANAGER);
            bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

            if (isOwnerRoleExists && isAdminRoleExists && isManagerRoleExists && isUserRoleExists)
            {
                return new GeneralServiceResponseDTO
                {
                    IsSucceed = true,
                    StatusCode = 200,
                    Message = "Roles seeding is already done"
                };
            }

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.MANAGER));
            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));

            return new GeneralServiceResponseDTO
            {
                IsSucceed = true,
                StatusCode = 200,
                Message = "Roles seeding done successfully"
            };
        }

        public async Task<GeneralServiceResponseDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDTO.UserName);

            if (isExistsUser is not null)
            {
                return new GeneralServiceResponseDTO()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "UserName aldready exists"
                };
            }

            var user = _mapper.Map<ApplicationUser>(registerDTO);

            var createUserResult = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!createUserResult.Succeeded)
            {
                var errorString = "User creation failed because:";

                foreach (var error in createUserResult.Errors)
                {
                    errorString += error;
                }

                return new GeneralServiceResponseDTO()
                {
                    IsSucceed = false,
                    StatusCode = 400,
                    Message = errorString
                };
            }

            // Add a default user role to all users
            await _userManager.AddToRoleAsync(user, StaticUserRoles.USER);
            await _logService.SaveNewLog(user.UserName, "Registered to website");

            return new GeneralServiceResponseDTO()
            {
                IsSucceed = true,
                StatusCode = 201,
                Message = "User created successfully"
            };
        }

        public async Task<LoginServiceResponseDTO?> LoginAsync(LoginDTO loginDTO)
        {
            // Find user with username
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);

            if (user is null)
            {
                return null;
            }

            // Check password of user
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isPasswordCorrect)
            {
                return null;
            }

            var token = await GenerateJWTTokenAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = GenerateUserInfoObject(user, roles);

            await _logService.SaveNewLog(user.UserName, "New Login");

            return new LoginServiceResponseDTO()
            {
                NewToken = token,
                UserInfoResult = userInfo,
            };
        }

        public async Task<GeneralServiceResponseDTO> UpdateRoleAsync(ClaimsPrincipal user, UpdateRoleDTO updateRoleDTO)
        {
            // Find user with username
            var isExistsUser = await _userManager.FindByNameAsync(updateRoleDTO.UserName);
            if (isExistsUser is null)
            {
                return new GeneralServiceResponseDTO()
                {
                    IsSucceed = false,
                    StatusCode = 404,
                    Message = "Invalid UserName"
                };
            }

            var userRoles = await _userManager.GetRolesAsync(isExistsUser);

            //Just the OWNER and ADMIN can update roles
            if (user.IsInRole(StaticUserRoles.ADMIN))
            {
                // User is admin
                if (updateRoleDTO.NewRole == RoleType.USER || updateRoleDTO.NewRole == RoleType.MANAGER)
                {
                    // Admin can change the role of everyone except for owners and admins
                    if (userRoles.Any(x => x.Equals(StaticUserRoles.OWNER) || x.Equals(StaticUserRoles.ADMIN)))
                    {
                        return new GeneralServiceResponseDTO()
                        {
                            IsSucceed = false,
                            StatusCode = 403,
                            Message = "You are not allowed to change role of this user"
                        };
                    }
                    else
                    {
                        await _userManager.RemoveFromRolesAsync(isExistsUser, userRoles);
                        await _userManager.AddToRoleAsync(isExistsUser, updateRoleDTO.NewRole.ToString());

                        await _logService.SaveNewLog(isExistsUser.UserName, "User roles updated");

                        return new GeneralServiceResponseDTO()
                        {
                            IsSucceed = true,
                            StatusCode = 200,
                            Message = "Role updated successfully"
                        };
                    }

                }
                else
                {
                    return new GeneralServiceResponseDTO()
                    {
                        IsSucceed = true,
                        StatusCode = 403,
                        Message = "You are not allowed to change role of this user"
                    };
                }
            }
            else
            {
                // User is owner
                if (userRoles.Any(x => x.Equals(StaticUserRoles.OWNER)))
                {
                    return new GeneralServiceResponseDTO()
                    {
                        IsSucceed = true,
                        StatusCode = 403,
                        Message = "You are not allowed to change role of this user"
                    };
                }
                else
                {
                    await _userManager.RemoveFromRolesAsync(isExistsUser, userRoles);
                    await _userManager.AddToRoleAsync(isExistsUser, updateRoleDTO.NewRole.ToString());

                    await _logService.SaveNewLog(isExistsUser.UserName, "User roles updated");

                    return new GeneralServiceResponseDTO()
                    {
                        IsSucceed = true,
                        StatusCode = 200,
                        Message = "Role updated successfully"
                    };
                }
            }
        }

        public async Task<LoginServiceResponseDTO?> MeAsync(MeDTO meDTO)
        {
            ClaimsPrincipal handler = new JwtSecurityTokenHandler()
                .ValidateToken(meDTO.Token, new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))

                }, out SecurityToken securityToken);

            string decodedUserName = handler.Claims.First(x => x.Type == ClaimTypes.Name).Value;

            if (decodedUserName is null)
            {
                return null;
            }

            var user = await _userManager.FindByNameAsync(decodedUserName);

            if (user == null)
            {
                return null;
            }

            var token = await GenerateJWTTokenAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = GenerateUserInfoObject(user, roles);

            await _logService.SaveNewLog(user.UserName, "New token generated");

            return new LoginServiceResponseDTO()
            {
                NewToken = token,
                UserInfoResult = userInfo
            };
        }

        public async Task<IEnumerable<UserInfoResult>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            List<UserInfoResult> userInfoResults = new();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = GenerateUserInfoObject(user, roles);
                userInfoResults.Add(userInfo);
            }

            return userInfoResults;
        }

        public async Task<UserInfoResult?> GetUserByUsernameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userInfor = GenerateUserInfoObject(user, roles);

            return userInfor;
        }

        public async Task<IEnumerable<string>> GetUserNamesAsync()
        {
            var userNames = await _userManager.Users.Select(x => x.UserName).ToListAsync();

            return userNames;
        }


        // GenerateJWTTokenAsync
        private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentitals = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                claims: authClaims,
                signingCredentials: signingCredentitals
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }


        // GenerateUserInfoObject
        // Can be replace with Automapper
        private UserInfoResult GenerateUserInfoObject(ApplicationUser user, IEnumerable<string> roles)
        {
            return new UserInfoResult()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = roles
            };
        }
    }
}
