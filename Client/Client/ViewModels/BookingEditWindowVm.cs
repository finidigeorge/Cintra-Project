using Common.DtoMapping;
using Mapping;
using RestClient;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Shared.Extentions;

using Client.Controls.WpfScheduler;
using Client.Commands;
using System.Collections.ObjectModel;
using Common;

namespace Client.ViewModels
{

    public class BookingEditWindowVm : BaseVm
    {
        BookingsClient bookingsClient = new BookingsClient();

        private BookingDtoUi _bookingData;
        public BookingDtoUi BookingData {
            get => _bookingData;
            set
            {
                Set(ref _bookingData, value, nameof(BookingData));
                _bookingData.ObjectLevelValidationCallback = BookingValidationCallback;
                _bookingData.PropertyChanged += async (s, e) =>
                {
                    if (_bookingData != null && e.PropertyName == nameof(_bookingData.BeginTime) || e.PropertyName == nameof(_bookingData.EndTime))
                    {
                        _bookingData.ValidationErrors = "";
                        await RunHorseValidations();
                        await RunCoachValidations();
                    }
                };

                RefreshAllModels();
            }
        }

        public bool DisplayOnlyAssignedCoaches { get; set; }

        public bool IsEditMode { get; set; }

        public bool ShowRecurringTab { get => !IsEditMode && (BookingData?.IsValid ?? false); }
        public bool ShowCancelRecurringTab { get => IsEditMode && BookingData?.BookingTemplateMetadata != null; }

        public bool EnableRecurringApointments { get; set; }
        public bool IsPermanent { get; set; }
        public bool EnableNonPermanentRecurringApointments { get => EnableRecurringApointments && !IsPermanent; }

        public int RecurringWeeksNumber { get; set; } = 10;
        public DateTime RecurringStartDate { get; set; } = DateTime.Now.TruncateToNextWeekday(DayOfWeek.Monday);

        public ICommand AddWeeklyScheduledIntervalCommand { get; set; }
        public ICommand UpdateWeeklyScheduledIntervalCommand { get; set; }
        public ICommand DeleteWeeklyScheduledIntervalCommand { get; set; }

        public ICommand GetServicesCommand { get => ServicesModel.RefreshDataCommand; }


        public ICommand AddCoachCommand { get; set; }
        public IAsyncCommand DeleteCoachCommand { get; set; }
        public bool CanDeleteCoach { get => CoachesVms.Count > 1; }

        public ICommand AddHorseCommand { get; set; }
        public IAsyncCommand DeleteHorseCommand { get; set; }
        public bool CanDeleteHorse { get => HorsesVms.Count > 1; }

        public ICommand AddClientCommand { get; set; }
        public IAsyncCommand DeleteClientCommand { get; set; }
        public bool CanDeleteClient { get => CoachesVms.Count > 1; }


        public Scheduler RecurrentScheduler { get; set; }

        public ServicesRefVm ServicesModel { get; set; } = new ServicesRefVm();        
        public PaymentTypesRefVm PaymentTypesModel { get; set; } = new PaymentTypesRefVm();

        private String horseValidationError;
        private String coachValidationError;

        public String HorseValidationHoursPerDayWarning { get; set; }
        public String HorseValidationHoursInRowWarning { get; set; }

        private StringBuilder BookingValidationCallback(StringBuilder error)
        {            
            if (!string.IsNullOrEmpty(horseValidationError))
                error.Append((error.Length != 0 ? ", " : "") + horseValidationError);

            if (!string.IsNullOrEmpty(coachValidationError))
                error.Append((error.Length != 0 ? ", " : "") + coachValidationError);

            return error;
        }

        public ObservableCollection<BookingEditCoachVm> CoachesVms { get; set; } = new ObservableCollection<BookingEditCoachVm>();
        public ObservableCollection<BookingEditHorseVm> HorsesVms { get; set; } = new ObservableCollection<BookingEditHorseVm>();
        public ObservableCollection<BookingEditClientVm> ClientsVms { get; set; } = new ObservableCollection<BookingEditClientVm>();

        public BookingEditWindowVm()
        {                        
            ServicesModel.OnSelectedItemChanged += async (sender, service) => {
                _bookingData.Service = service;

                foreach (var c in HorsesVms)
                    await c.GetHorsesCommand.ExecuteAsync(null);                

                foreach(var c in CoachesVms)
                    await c.GetCoachesCommand.ExecuteAsync(c.DisplayOnlyAssignedCoaches ? service.Id : (long?)null);

                _bookingData.EndTime = _bookingData.BeginTime.AddMinutes((int)service.LengthMinutes);
                _bookingData.OnPropertyChanged("EndTime");
                OnPropertyChanged(nameof(ShowRecurringTab));
            };
                        
            PaymentTypesModel.OnSelectedItemChanged += (sender, paymentType) => { _bookingData.BookingPayment.PaymentType = paymentType; };
            

            AddCoachCommand = new Command<object>(() =>
            {
                CoachesVms.Add(new BookingEditCoachVm(this));
            }, (x) => CoachesVms.Count < 5);

            DeleteCoachCommand = new AsyncCommand<Guid>(async (param) =>
            {
                CoachesVms.Remove(CoachesVms.First(x => x.Id == param));                
            }, (x) => CanDeleteCoach);

            
            AddHorseCommand = new Command<object>(() =>
            {
                HorsesVms.Add(new BookingEditHorseVm(this));
            }, (x) => HorsesVms.Count < 5);

            DeleteHorseCommand = new AsyncCommand<Guid>(async (param) =>
            {
                HorsesVms.Remove(HorsesVms.First(x => x.Id == param));
            }, (x) => CanDeleteHorse);

            AddClientCommand = new Command<object>(() =>
            {
                ClientsVms.Add(new BookingEditClientVm(this));
            }, (x) => ClientsVms.Count < 5);

            DeleteClientCommand = new AsyncCommand<Guid>(async (param) =>
            {
                ClientsVms.Remove(ClientsVms.First(x => x.Id == param));
            }, (x) => CanDeleteClient);


            AddCoachCommand.Execute(null);
            AddHorseCommand.Execute(null);
            AddClientCommand.Execute(null);
        }

        public async Task RunCoachValidations()
        {
            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            coachValidationError = null;

            if (dto.Coaches.Any(x => x != null))
            {
                var res = await bookingsClient.HasCoachesNotOverlappedBooking(dto);
                if (!res.Result)
                    coachValidationError = res.ErrorMessage;

                res = await bookingsClient.HasCoachesScheduleFitBooking(dto);
                if (!res.Result)
                    coachValidationError = coachValidationError + (!string.IsNullOrEmpty(coachValidationError) ? ", " : "") + res.ErrorMessage;
            }

            //to raise validation checks
            _bookingData.OnPropertyChanged(nameof(_bookingData.Coaches));
            OnPropertyChanged(nameof(ShowRecurringTab));
        }

        public async Task RunHorseValidations()
        {
            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            if (dto.Horses?.Any() ?? false)
            {
                horseValidationError = null;
                HorseValidationHoursPerDayWarning = null;
                HorseValidationHoursInRowWarning = null;

                var res = await bookingsClient.HasHorsesNotOverlappedBooking(dto);
                if (!res.Result)
                    horseValidationError = res.ErrorMessage;

                res = await bookingsClient.HasHorsesScheduleFitBooking(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError + (!string.IsNullOrEmpty(horseValidationError) ? ", " : "") + res.ErrorMessage;

                res = await bookingsClient.HasHorsesRequiredBreak(dto);
                if (!res.Result)
                    HorseValidationHoursInRowWarning = res.ErrorMessage;

                res = await bookingsClient.HasHorsesWorkedLessThanAllowed(dto);
                if (!res.Result)
                    HorseValidationHoursPerDayWarning = res.ErrorMessage;

            }

            //to raise validation checks
            _bookingData.OnPropertyChanged(nameof(_bookingData.Horses));
            OnPropertyChanged(nameof(ShowRecurringTab));
        }

        private async void RefreshAllModels()
        {
            if (BookingData.BookingPayment?.PaymentType != null)
            {
                PaymentTypesModel.SelectedItem = PaymentTypesModel.Items.FirstOrDefault(x => x.Id == BookingData.BookingPayment.PaymentType.Id);
            }

            await PaymentTypesModel.RefreshDataCommand.ExecuteAsync(null);
        }        
    }
}
