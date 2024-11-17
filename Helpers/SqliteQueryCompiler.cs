using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SqlKata.Compilers;
using trabahuso_api.Interfaces;

namespace trabahuso_api.Helpers
{
    public class SqliteQueryCompiler : SqliteCompiler, ISqliteQueryCompiler
    {
        public override SqlResult Compile(Query query)
        {
            return base.Compile(query);
        }
    }
}