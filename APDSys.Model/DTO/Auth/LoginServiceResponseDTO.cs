using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.DTO.Auth
{
    public class LoginServiceResponseDTO
    {
        public string NewToken { get; set; }
        
        // This would be returned to front-end
        public UserInfoResult UserInfoResult { get; set; }
    }
}
