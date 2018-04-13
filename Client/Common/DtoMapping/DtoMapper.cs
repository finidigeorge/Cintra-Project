using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;
using PropertyChanged;
using Shared.Extentions;
using Mapping;
using Shared;
using Newtonsoft.Json;


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

        public Func<StringBuilder, StringBuilder> ObjectLevelValidationCallback;

        [DependsOn(nameof(BeginTime))]
        public String BeginTimeFmtd => BeginTime.ToString("hh:mm tt");

        [DependsOn(nameof(BeginTime))]
        public String BeginTimeRoundedFmtd => BeginTime.RoundDown(TimeSpan.FromMinutes(60)).ToString("hh:mm tt");

        [DependsOn(nameof(BeginTime), nameof(EndTime))]
        public String LengthFmtd {
            get
            {
                if (BeginTime > EndTime) { return "0"; }

                return (EndTime - BeginTime).ToString("h'h 'm'm '");
            }
        }

        public string ApplyObjectLevelValidations()
        {
            StringBuilder error = new StringBuilder();

            if (!string.IsNullOrEmpty(ValidationErrors))
            {
                error.Append(ValidationErrors);
                return error.ToString();
            }

            if (BeginTime >= EndTime)
            {
                error.Append((error.Length != 0 ? ", " : "") + "Begin time and End time values are incorrect");
            }

            if (ObjectLevelValidationCallback != null)
            {
                error = ObjectLevelValidationCallback(error);
            }

            return error.ToString();
        }

        [DependsOn(nameof(HasValidationWarnings))]
        public bool HasValidationWarnings => !String.IsNullOrEmpty(ValidationWarnings);

        [DependsOn(nameof(HasValidationWarnings), nameof(IsValid))]
        public int Status {
            get
            {
                if (!IsValid)
                    return -1;

                if (IsValid && !HasValidationWarnings)
                    return 0;

                if (IsValid && HasValidationWarnings)
                    return 1;

                throw new Exception("Unknown booking status");
            }
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
            StringBuilder error = new StringBuilder();

            if (CoachRole == null)
            {
                error.Append((error.Length != 0 ? ", " : "") + "Staff Role should not be empty");
            }

            return error.ToString();
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

        [DependsOn(nameof(Name), nameof(Phone), nameof(Email))]
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
            StringBuilder error = new StringBuilder();
            
            if (LengthMinutes == null)               
            {
                error.Append((error.Length != 0 ? ", " : "") + "Length in minutes should not be empty");
            }

            return error.ToString();
        }

        [DependsOn(nameof(Coaches))]
        public String SelectedCoaches {
            get
            {
                return String.Join(", ", Coaches.Select(x => x.Name));
            }
        }

        [DependsOn(nameof(Horses))]
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

    public partial class HorseScheduleDataDtoUi : HorseScheduleDataDto, INotifyPropertyChanged
    {
        [DependsOn(nameof(UnavailabilityType))]
        public int UnavailabilityReasonMapped {
            get => (int)UnavailabilityType;
            set
            {
                UnavailabilityType = (HorsesUnavailabilityEnum)value;
            }
        }

        public bool IsDateInterval { get; set; }

        [DependsOn(nameof(IsDateInterval))]
        public bool IsDayOfWeek { get => !IsDateInterval; }

        [DependsOn(nameof(DayOfWeek))]
        public string DayOfWeekStr { get => DayOfWeek.HasValue ? DateTimeExtentions.DayNumberToString((int)DayOfWeek.Value) : String.Empty; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string ApplyObjectLevelValidations()
        {
            StringBuilder error = new StringBuilder();

            if (StartDate >= EndDate)
            {
                error.Append((error.Length != 0 ? ", " : "") + "Date To / Date From values are incorrect");
            }

            return error.ToString();
        }

        public override string ToString()
        {
            if (DayOfWeek.HasValue)
                return $"{UnavailabilityType.ToString()} {((DayOfWeek)(DayOfWeek.Value + 1 < 7 ? DayOfWeek.Value + 1 : 0))}";

            else 
                return $"{UnavailabilityType.ToString()} {StartDate.Value.ToString("dd/MM/yyyy")} {EndDate.Value.ToString("dd/MM/yyyy")}";
        }
    }

#pragma warning restore CS0067 
}
