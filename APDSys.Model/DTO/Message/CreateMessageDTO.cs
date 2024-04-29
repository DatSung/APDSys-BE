using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.DTO.Message
{
    public class CreateMessageDTO
    {
        public string ReceiverUserName { get; set; }
        public string Text { get; set; }
    }
}
