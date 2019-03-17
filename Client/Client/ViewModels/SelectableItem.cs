using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class SelectableItem : BaseVm
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
