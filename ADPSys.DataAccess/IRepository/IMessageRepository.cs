using APDSys.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPSys.DataAccess.IRepository
{
    public interface IMessageRepository : IRepository<Message>
    {
        void Update(Message message);
        void UpdateRange(List<Message> messages);
    }
}
