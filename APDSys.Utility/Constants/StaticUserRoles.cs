using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Utility.Constants
{
    // This class will be used to avoid typing error
    public class StaticUserRoles
    {
        public const string OWNER = "OWNER";
        public const string ADMIN = "ADMIN";
        public const string MANAGER = "MANAGER";
        public const string USER = "USER";

        public const string OwnerAdmin = "OWNER,ADMIN";
        public const string OwnerAdminManager = "OWNER,ADMIN,MANAGER";
        public const string OwnerAdminManagerUSer = "OWNER,ADMIN,MANAGER,USER";
    }
}
