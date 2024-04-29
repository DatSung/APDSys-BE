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
    public class LogRepository : Repository<Log>, ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Log log)
        {
            _context.Update(log);
        }

        public void UpdateRange(List<Log> logs)
        {

            _context.UpdateRange(logs);
        }

    }
}
