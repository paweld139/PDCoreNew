using PDCore.Repositories.IRepo;
using PDCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCore.Repositories.IRepo
{
    public interface ILogRepo : ISqlRepositoryEntityFramework<LogModel>
    {

    }
}
