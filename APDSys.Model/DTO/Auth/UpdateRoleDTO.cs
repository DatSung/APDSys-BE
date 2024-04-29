using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.DTO.Auth
{
    public class UpdateRoleDTO
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        public RoleType NewRole { get; set; }
    }

    public enum RoleType
    {
        ADMIN,
        MANAGER,
        USER
    }
}
