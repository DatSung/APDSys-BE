using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.DTO.General
{
    public class GeneralServiceResponseDTO
    {
        public bool IsSucceed { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
