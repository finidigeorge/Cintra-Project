using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;


/*
    Wrappers for Dto classes with INotifyPropertyChanged support. 
    The actual impelemntation of INotifyPropertyChanged is done by magic of Fody (https://github.com/Fody/PropertyChanged)
*/
namespace Common.DtoMapping
{
      
#pragma warning disable CS0067
    public class CoachDtoUi : CoachDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }

    public class HorseDtoUi : HorseDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }

    public class ServiceDtoUi : ServiceDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }
    public class UserDtoUi : UserDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }
    public class UserRoleDtoUi : UserRoleDto, INotifyPropertyChanged { public event PropertyChangedEventHandler PropertyChanged; }
#pragma warning restore CS0067 
}
