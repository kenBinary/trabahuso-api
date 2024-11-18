using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.Models;

namespace trabahuso_api.Mapper
{
    public static class SqlArgumentMapper
    {
        public static List<SqlArgument> ToSqlArguments(this List<object> bindings)
        {
            List<SqlArgument> arguments = [];

            foreach (var binding in bindings)
            {
                string argumentType;

                if (binding is null)
                {
                    argumentType = "null";
                }
                else if (
                    binding is int || binding is ulong || binding is short ||
                    binding is byte || binding is sbyte || binding is ushort ||
                    binding is uint || binding is long
                )
                {
                    argumentType = "integer";
                }
                else if (
                    binding is float || binding is double || binding is decimal
                )
                {
                    argumentType = "float";
                }
                else
                {
                    argumentType = "text";
                }

                var argument = new SqlArgument(
                    argumentType,
                    $"{binding}"
                );

                arguments.Add(argument);
            }

            return arguments;
        }
    }
}