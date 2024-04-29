using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.Domain
{
    public class Log : BaseEntity<long>
    {
        public string? UserName { get; set; }
        public string Description { get; set; }
    }
}
