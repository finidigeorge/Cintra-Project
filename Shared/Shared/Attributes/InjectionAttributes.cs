using System;

namespace Shared.Attributes
{    
    public class Chained : Attribute
    {
    }

    public class Singleton : Attribute
    {
        public bool InstantiateOnStartup { get; set; }

        public bool IsolateScope { get; set; }
    }

    public class PerScope : Attribute
    {
    }    
}
