using ADPSys.DataAccess.Context;
using ADPSys.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPSys.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ILogRepository LogRepository { get; set; }
        public IMessageRepository MessageRepository { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            LogRepository = new LogRepository(_context);
            MessageRepository = new MessageRepository(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
