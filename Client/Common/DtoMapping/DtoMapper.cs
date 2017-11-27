﻿using System;
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

    public partial class BookingDtoUi : BookingDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        public string ApplyObjectLevelValidations()
        {
            StringBuilder error = new StringBuilder();

            if (BeginTime >= EndTime)
            {
                error.Append((error.Length != 0 ? ", " : "") + "Begin time and End time values are incorrect");
            }

            return error.ToString();
        }
    }

    public partial class BookingPaymentDtoUi : BookingPaymentDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class CoachDtoUi : CoachDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;        

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }

        public override string ToString() => Name;
    }

    public partial class ClientDtoUi : ClientDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }

        public override string ToString() => $"{Name} ph/email: {Phone ?? Email}";
    }

    public partial class ScheduleDtoUi : ScheduleDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public new List<ScheduleDataDto> ScheduleData { get; set; }

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class ScheduleDataDtoUi : ScheduleDataDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApplyObjectLevelValidations()
        {
            StringBuilder error = new StringBuilder();

            if (BeginTime >= EndTime)
            {
                error.Append((error.Length != 0 ? ", " : "") + "Begin time and End time values are incorrect");
            }

            return error.ToString();
        }
    }

    public partial class HorseDtoUi : HorseDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }

        public override string ToString() => NickName;
    }

    public partial class ServiceDtoUi : ServiceDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }

        public override string ToString() => Name;
    }

    public partial class UserDtoUi : UserDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }

    public partial class UserRoleDtoUi : UserRoleDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }
    }
#pragma warning restore CS0067 
}
