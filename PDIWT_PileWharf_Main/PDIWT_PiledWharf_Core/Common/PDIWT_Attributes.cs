using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDIWT_PiledWharf_Core.Common
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =false)]
    public class ECAttribute : Attribute
    {
        public string SchemaName { get; set; }
        public string ClassName { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
    }
}
