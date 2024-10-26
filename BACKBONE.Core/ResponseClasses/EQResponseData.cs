using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACKBONE.Core.ResponseClasses
{
    public class EQResponseData<T>
    {
        public T? SingleValue { get; set; }
        public List<T>? ListValue { get; set; }
        public DataTable? DataTableValue { get; set; }
    }
}
