using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADPSys.DataAccess.IRepository
{
    public interface IUnitOfWork
    {
        ILogRepository LogRepository { get; }
        IMessageRepository MessageRepository { get; }

        Task SaveAsync();
    }
}
