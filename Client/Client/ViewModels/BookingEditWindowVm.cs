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
using Shared.Dto.Interfaces;
using Client.Extentions;
using System.Windows.Threading;

namespace Client.ViewModels
{

    public class BookingEditWindowVm : BaseVm
    {
        private int MaxClients { get => _bookingData?.Service == null ? 6 : (int)_bookingData?.Service.MaxClients; }
        private int MaxHorses { get => _bookingData?.Service == null ? 6 : (int)_bookingData?.Service.MaxHorses; }
        private int MaxCoaches { get => _bookingData?.Service == null ? 6 : (int)_bookingData?.Service.MaxCoaches; }

        BookingsClient bookingsClient = new BookingsClient();

        private BookingDtoUi _bookingData;
        private bool enableChecks = true;

        private void SetUpLinkedDtoCollection<T>(ICommand command, List<T> collection) where T: IUniqueDto
        {
            if ((collection?.Any() ?? false))
                collection.ForEach(x => command.Execute(x));
            else
                command.Execute(null);
        }

        public BookingDtoUi BookingData {
            get => _bookingData;
            set
            {
                Set(ref _bookingData, value, nameof(BookingData));

                if (IsEditMode)
                {
                    enableChecks = false;

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(2000);
                    timer.Tick += (s, e) =>
                    {
                        enableChecks = true;
                    };

                    timer.Start();
                }

                _bookingData.ObjectLevelValidationCallback = BookingValidationCallback;
                _bookingData.PropertyChanged += async (s, e) =>
                {
                    if (_bookingData != null && e.PropertyName == nameof(_bookingData.BeginTime) || e.PropertyName == nameof(_bookingData.EndTime))
                    {
                        _bookingData.ValidationErrors = "";
                        await RunHorseValidations();
                        await RunCoachValidations();
                        RunClientValidations();
                    }
                };

                RefreshAllModels();

                SetUpLinkedDtoCollection(AddCoachCommand, _bookingData?.Coaches);
                SetUpLinkedDtoCollection(AddHorseCommand, _bookingData?.Horses);
                SetUpLinkedDtoCollection(AddClientCommand, _bookingData?.Clients);                                       
            }
        }

        public bool DisplayOnlyAssignedCoaches { get; set; }

        public bool IsEditMode { get; set; }
        
        public ICommand GetServicesCommand { get => ServicesModel.RefreshDataCommand; }

        public ICommand AddCoachCommand { get; set; }
        public ICommand SyncCoachesCommand { get; set; }
        public ICommand DeleteCoachCommand { get; set; }
        public bool CanDeleteCoach { get => CoachesVms.Count > 1; }

        public ICommand AddHorseCommand { get; set; }
        public ICommand SyncHorsesCommand { get; set; }
        public ICommand DeleteHorseCommand { get; set; }
        public bool CanDeleteHorse { get => HorsesVms.Count > 1; }

        public ICommand AddClientCommand { get; set; }
        public ICommand SyncClientsCommand { get; set; }
        public ICommand DeleteClientCommand { get; set; }
        public bool CanDeleteClient { get => ClientsVms.Count > 1; }        

        public ServicesRefVm ServicesModel { get; set; } = new ServicesRefVm();        
        public PaymentTypesRefVm PaymentTypesModel { get; set; } = new PaymentTypesRefVm();

        private String horseValidationError;
        private String coachValidationError;
        private String clientValidationError;


        public String CoachValidationFitScheduleWarning { get; set; }
        public String HorseValidationHoursPerDayWarning { get; set; }
        public String HorseValidationHoursInRowWarning { get; set; }

        private StringBuilder BookingValidationCallback(StringBuilder error)
        {            
            if (!string.IsNullOrEmpty(horseValidationError))
                error.Append((error.Length != 0 ? ", " : "") + horseValidationError);

            if (!string.IsNullOrEmpty(coachValidationError))
                error.Append((error.Length != 0 ? ", " : "") + coachValidationError);

            if (!string.IsNullOrEmpty(clientValidationError))
                error.Append((error.Length != 0 ? ", " : "") + clientValidationError);

            return error;
        }

        public ObservableCollection<BookingEditCoachVm> CoachesVms { get; set; } = new ObservableCollection<BookingEditCoachVm>();
        public ObservableCollection<BookingEditHorseVm> HorsesVms { get; set; } = new ObservableCollection<BookingEditHorseVm>();
        public ObservableCollection<BookingEditClientVm> ClientsVms { get; set; } = new ObservableCollection<BookingEditClientVm>();

        #region Permanent Booking
        public Scheduler RecurrentScheduler { get; set; }

        public int WeekNumber { get; set; }

        public bool ShowRecurringTab { get => !IsEditMode && (BookingData?.IsValid ?? false); }
        public bool ShowCancelRecurringTab { get => IsEditMode && BookingData?.BookingTemplateMetadata != null; }

        public bool EnableRecurringApointments { get; set; }
        public bool IsPermanent { get; set; }        
        public bool EnableNonPermanentRecurringApointments { get => EnableRecurringApointments && !IsPermanent; }

        public bool IsFortnightly { get; set; }
        public bool SchedulerPrevNextWeekButtonsVisible { get => EnableRecurringApointments && IsFortnightly; }

        public int RecurringWeeksNumber { get; set; } = 10;
        public DateTime RecurringStartDate { get; set; } = DateTime.Now.TruncateToNextWeekday(DayOfWeek.Monday);

        public ICommand AddWeeklyScheduledIntervalCommand { get; set; }
        public ICommand UpdateWeeklyScheduledIntervalCommand { get; set; }
        public ICommand DeleteWeeklyScheduledIntervalCommand { get; set; }


        public ICommand NextWeekCommand { get; set; }
        public ICommand PrevWeekCommand { get; set; }

        #endregion

        public BookingEditWindowVm()
        {
            ServicesModel.OnSelectedItemChanged += async (sender, service) =>
            {
                _bookingData.Service = service;

                foreach (var c in HorsesVms)
                    await c.GetHorsesCommand.ExecuteAsync(null);

                foreach (var c in CoachesVms)
                    await c.GetCoachesCommand.ExecuteAsync(c.DisplayOnlyAssignedCoaches ? service?.Id : (long?)null);

                if (service == null)
                    return;

                _bookingData.EndTime = _bookingData.BeginTime.AddMinutes((int)service.LengthMinutes);
                _bookingData.OnPropertyChanged("EndTime");
                OnPropertyChanged(nameof(ShowRecurringTab));
            };

            PaymentTypesModel.OnSelectedItemChanged += (sender, paymentType) => { _bookingData.BookingPayment.PaymentType = paymentType; };

            SetCommands();
        }

        private void SetCommands()
        {
            AddCoachCommand = new Command<object>((param) =>
            {
                var coachDto = ObjectMapper.Map<CoachDtoUi>(param);
                var c = new BookingEditCoachVm(this, coachDto);
                CoachesVms.Add(c);                
            }, (x) => CoachesVms.Count < MaxCoaches);
            SyncCoachesCommand = new Command<object>((param) =>
            {
                if (_bookingData.Coaches == null)
                    _bookingData.Coaches = new List<CoachDto>();
                else
                    _bookingData.Coaches.Clear();

                foreach (var c in CoachesVms)
                    if (c.Model.SelectedItem != null)
                        _bookingData.Coaches.Add(c.Model.SelectedItem);

            }, (x) => CoachesVms.Count > 0);
            DeleteCoachCommand = new Command<Guid>((param) =>
            {
                var c = CoachesVms.First(x => x.Id == param);
                _bookingData.Coaches.Remove(c.Model.SelectedItem);

                CoachesVms.Remove(c);
            }, (x) => CanDeleteCoach);


            AddHorseCommand = new Command<object>((param) =>
            {
                var horseDto = ObjectMapper.Map<HorseDtoUi>(param);
                var h = new BookingEditHorseVm(this, horseDto);
                HorsesVms.Add(h);                
            }, (x) => HorsesVms.Count < MaxHorses && (!_bookingData?.Service?.NoHorseRequired ?? false));
            SyncHorsesCommand = new Command<object>((param) =>
            {
                if (_bookingData.Horses == null)
                    _bookingData.Horses = new List<HorseDto>();
                else
                    _bookingData.Horses.Clear();

                foreach (var c in HorsesVms)
                    if (c.Model.SelectedItem != null)
                        _bookingData.Horses.Add(c.Model.SelectedItem);

            }, (x) => HorsesVms.Count > 0);
            DeleteHorseCommand = new Command<Guid>((param) =>
            {
                var c = HorsesVms.First(x => x.Id == param);
                _bookingData.Horses.Remove(c.Model.SelectedItem);
                HorsesVms.Remove(c);
            }, (x) => CanDeleteHorse);

            AddClientCommand = new Command<object>((param) =>
            {
                var clientDto = ObjectMapper.Map<ClientDtoUi>(param);
                var c = new BookingEditClientVm(this, clientDto);                
                ClientsVms.Add(c);

            }, (x) => ClientsVms.Count < MaxClients);
            SyncClientsCommand = new Command<object>((param) =>
            {
                if (_bookingData.Clients == null)
                    _bookingData.Clients = new List<ClientDto>();
                else
                    _bookingData.Clients.Clear();

                foreach (var c in ClientsVms)
                    if (c.Model.SelectedItem != null)
                        _bookingData.Clients.Add(c.Model.SelectedItem);

            }, (x) => ClientsVms.Count > 0);
            DeleteClientCommand = new Command<Guid>((param) =>
            {
                var c = ClientsVms.First(x => x.Id == param);
                _bookingData.Clients.Remove(c.Model.SelectedItem);
                ClientsVms.Remove(c);
            }, (x) => CanDeleteClient);
        }        

        public async Task RunCoachValidations()
        {
            if (!enableChecks)
                return;

            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            coachValidationError = null;
            CoachValidationFitScheduleWarning = null;

            if (dto.Coaches.Any(x => x != null))
            {                
                var res = await bookingsClient.HasCoachesNotOverlappedBooking(dto);
                if (!res.Result)
                    coachValidationError = res.ErrorMessage;

                res = await bookingsClient.HasCoachesScheduleFitBooking(dto);
                if (!res.Result)                    
                    coachValidationError = coachValidationError.Append(res.ErrorMessage);

                if (HasDuplicates(dto.Coaches))
                    coachValidationError = coachValidationError.Append("Booking has duplicate coaches");

                res = await bookingsClient.HasCoachScheduleFitBreaks(dto);
                if (!res.Result)
                    CoachValidationFitScheduleWarning = res.ErrorMessage;
            }

            //to raise validation checks
            _bookingData.OnPropertyChanged(nameof(_bookingData.Coaches));
            OnPropertyChanged(nameof(ShowRecurringTab));
        }

        public async Task RunHorseValidations()
        {            
            if (!enableChecks || (_bookingData?.Service?.NoHorseRequired ?? false))
                return;

            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            if (dto.Horses?.Any() ?? false)
            {
                horseValidationError = null;
                HorseValidationHoursPerDayWarning = null;
                HorseValidationHoursInRowWarning = null;

                //Errors
                if (HasDuplicates(dto.Horses))
                    horseValidationError = "Booking has duplicate horses";

                var res = await bookingsClient.HasHorsesNotOverlappedBooking(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError.Append(res.ErrorMessage);

                res = await bookingsClient.HasHorsesScheduleFitBooking(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError.Append(res.ErrorMessage);

                res = await bookingsClient.HasHorseAssignedToAtLeastOneOfCoaches(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError.Append(res.ErrorMessage);

                //Warnings
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

        public bool HasDuplicates<T>(List<T> collection) where T : IUniqueDto
        {
            return collection.GroupBy(x => x.Id).Any(group => group.Count() > 1);
        }

        public void RunClientValidations()
        {
            clientValidationError = null;

            if (_bookingData?.Clients?.Any() ?? false)
            {
                if (HasDuplicates(_bookingData.Clients))
                    clientValidationError = "Booking has duplicate clients";
            }            

            //to raise validation checks
            _bookingData.OnPropertyChanged(nameof(_bookingData.Clients));
            OnPropertyChanged(nameof(ShowRecurringTab));
        }

        private async void RefreshAllModels()
        {
            await ServicesModel.RefreshDataCommand.ExecuteAsync(null);
            if (BookingData.Service != null)
            {
                ServicesModel.SelectedItem = ServicesModel.Items.FirstOrDefault(x => x.Id == BookingData.Service.Id);
            }

            await PaymentTypesModel.RefreshDataCommand.ExecuteAsync(null);
            if (BookingData.BookingPayment?.PaymentType != null)
            {
                PaymentTypesModel.SelectedItem = PaymentTypesModel.Items.FirstOrDefault(x => x.Id == BookingData.BookingPayment.PaymentType.Id);
            }

            foreach (var c in ClientsVms)
                await c.GetClientsCommand.ExecuteAsync(null);
        }        
    }
}
