using ADPSys.DataAccess.Context;
using ADPSys.DataAccess.IRepository;
using APDSys.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPSys.DataAccess.Repository
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Message message)
        {
            _context.Update(message);
        }

        public void UpdateRange(List<Message> messages)
        {
            _context.UpdateRange(messages);
        }
    }
}
