using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Attributes
{
    public class VmMetaAttribute: Attribute
    {
        public bool IsReadonly { get; set; }

        public bool IsNullable { get; set; } = true;

        public int Min { get; set; }

        public int Max { get; set; }

    }    
}
