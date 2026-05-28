using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RenewalTransFiles.Models
{
    public class SqlConnConfig
    {
        public SqlConnConfig(string value) => Value = value;

        public string Value { get; }
    }
}
