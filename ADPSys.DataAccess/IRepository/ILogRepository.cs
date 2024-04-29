using APDSys.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPSys.DataAccess.IRepository
{
    public interface ILogRepository : IRepository<Log>
    {
        void Update(Log log);
        void UpdateRange(List<Log> logs);
    }
}
