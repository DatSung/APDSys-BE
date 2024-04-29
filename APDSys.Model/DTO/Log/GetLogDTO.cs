using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.DTO.Log
{
    public class GetLogDTO
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? UserName { get; set; }
        public string? Description { get; set; }

    }
}
