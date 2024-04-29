using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APDSys.Model.DTO.Message
{
    public class GetMessageDTO
    {
        public long Id { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
