using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;


/*
    Wrappers for Dto classes with INotifyPropertyChanged support. 
    The actual impelentation of INotifyPropertyChanged is done by magic of Fody (https://github.com/Fody/PropertyChanged)
*/
namespace Common.DtoMapping
{
    public interface IAtomicEditableObject : IEditableObject 
    {        
        event ItemEndEditEventHandler ItemEndEdit;
    }

    public delegate void ItemEndEditEventHandler(IAtomicEditableObject sender);

#pragma warning disable CS0067
    public partial class CoachDtoUi : CoachDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }

    public partial class HorseDtoUi : HorseDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }

    public partial class ServiceDtoUi : ServiceDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }
    public partial class UserDtoUi : UserDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }
    public partial class UserRoleDtoUi : UserRoleDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }
#pragma warning restore CS0067 
}
