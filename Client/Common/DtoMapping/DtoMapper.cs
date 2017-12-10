using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;
using PropertyChanged;
using Common.Extentions;
using Mapping;


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

        [DependsOn("BeginTime")]
        public String BeginTimeFmtd => BeginTime.ToString("hh:mm tt");

        [DependsOn("BeginTime")]
        public String BeginTimeRoundedFmtd => BeginTime.RoundDown(TimeSpan.FromMinutes(60)).ToString("hh:mm tt");

        [DependsOn("BeginTime", "EndTime")]
        public String LengthFmtd {
            get
            {
                if (BeginTime > EndTime) { return "0"; }

                return (EndTime - BeginTime).ToString("h'h 'm'm '");
            }
        }

        [DependsOn("Client")]
        public ClientDtoUi ClientUi => ObjectMapper.Map<ClientDtoUi>(Client);

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

        [DependsOn("Name", "Phone", "Email")]
        public String NameFmtd => $"{Name}, ph/@: {Phone ?? Email}";

        public override string ToString() => NameFmtd;
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

        [DependsOn("Coaches")]
        public String SelectedCoaches {
            get
            {
                return String.Join(", ", Coaches.Select(x => x.Name));
            }
        }

        [DependsOn("Horses")]
        public String SelectedHorses
        {
            get
            {
                return String.Join(", ", Horses.Select(x => x.NickName));
            }
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

        public override string ToString() => Name;
    }

    public partial class UserRoleDtoUi : UserRoleDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }

        public override string ToString() => Name;
    }


    public partial class PaymentTypeDtoUi : PaymentTypeDto, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ApplyObjectLevelValidations()
        {
            return string.Empty;
        }

        public override string ToString() => Name;
    }
#pragma warning restore CS0067 
}
