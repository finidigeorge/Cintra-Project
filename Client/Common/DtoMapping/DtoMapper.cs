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
        event ItemEndCancelEventHandler ItemCancelEdit;
    }

    public interface ICustomDataErrorInfo : IDataErrorInfo
    {
        string ApplyObjectLevelValidations();
    }

    public delegate void ItemEndEditEventHandler(IAtomicEditableObject sender);
    public delegate void ItemEndCancelEventHandler(IAtomicEditableObject sender);

#pragma warning disable CS0067
    public partial class CoachDtoUi : CoachDto, INotifyPropertyChanged, ICustomDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class HorseDtoUi : HorseDto, INotifyPropertyChanged, ICustomDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class ServiceDtoUi : ServiceDto, INotifyPropertyChanged, ICustomDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class UserDtoUi : UserDto, INotifyPropertyChanged, ICustomDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class UserRoleDtoUi : UserRoleDto, INotifyPropertyChanged, ICustomDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }
#pragma warning restore CS0067 
}
