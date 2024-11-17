using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;

namespace trabahuso_api.Interfaces
{
    public interface ISqliteQueryCompiler
    {
        SqlResult Compile(Query query);
    }
}