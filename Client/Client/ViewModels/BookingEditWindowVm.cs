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
        const int MAX_CLIENTS = 6;
        BookingsClient bookingsClient = new BookingsClient();

        private BookingDtoUi _bookingData;
        private bool enableChecks = true;

        private void SetUpLinkedDtoCollection<T>(IAsyncCommand command, List<T> collection) where T: IUniqueDto
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


        public IAsyncCommand AddCoachCommand { get; set; }
        public ICommand SyncCoachesCommand { get; set; }
        public IAsyncCommand DeleteCoachCommand { get; set; }
        public bool CanDeleteCoach { get => CoachesVms.Count > 1; }

        public IAsyncCommand AddHorseCommand { get; set; }
        public ICommand SyncHorsesCommand { get; set; }
        public IAsyncCommand DeleteHorseCommand { get; set; }
        public bool CanDeleteHorse { get => HorsesVms.Count > 1; }

        public IAsyncCommand AddClientCommand { get; set; }
        public ICommand SyncClientsCommand { get; set; }
        public IAsyncCommand DeleteClientCommand { get; set; }
        public bool CanDeleteClient { get => ClientsVms.Count > 1; }


        public Scheduler RecurrentScheduler { get; set; }

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
            AddCoachCommand = new AsyncCommand<object>(async (param) =>
            {
                var coachDto = ObjectMapper.Map<CoachDtoUi>(param);
                var c = new BookingEditCoachVm(this, coachDto);
                CoachesVms.Add(c);                
            }, (x) => CoachesVms.Count < MAX_CLIENTS);
            SyncCoachesCommand = new Command<object>(() =>
            {
                if (_bookingData.Coaches == null)
                    _bookingData.Coaches = new List<CoachDto>();
                else
                    _bookingData.Coaches.Clear();

                foreach (var c in CoachesVms)
                    if (c.Model.SelectedItem != null)
                        _bookingData.Coaches.Add(c.Model.SelectedItem);

            }, (x) => CoachesVms.Count > 0);
            DeleteCoachCommand = new AsyncCommand<Guid>(async (param) =>
            {
                var c = CoachesVms.First(x => x.Id == param);
                _bookingData.Coaches.Remove(c.Model.SelectedItem);

                await Task.FromResult(CoachesVms.Remove(c));
            }, (x) => CanDeleteCoach);


            AddHorseCommand = new AsyncCommand<object>(async (param) =>
            {
                var horseDto = ObjectMapper.Map<HorseDtoUi>(param);
                var h = new BookingEditHorseVm(this, horseDto);
                HorsesVms.Add(h);                
            }, (x) => HorsesVms.Count < MAX_CLIENTS);
            SyncHorsesCommand = new Command<object>(() =>
            {
                if (_bookingData.Horses == null)
                    _bookingData.Horses = new List<HorseDto>();
                else
                    _bookingData.Horses.Clear();

                foreach (var c in HorsesVms)
                    if (c.Model.SelectedItem != null)
                        _bookingData.Horses.Add(c.Model.SelectedItem);

            }, (x) => HorsesVms.Count > 0);
            DeleteHorseCommand = new AsyncCommand<Guid>(async (param) =>
            {
                var c = HorsesVms.First(x => x.Id == param);
                _bookingData.Horses.Remove(c.Model.SelectedItem);

                await Task.FromResult(HorsesVms.Remove(c));
            }, (x) => CanDeleteHorse);

            AddClientCommand = new AsyncCommand<object>(async (param) =>
            {
                var clientDto = ObjectMapper.Map<ClientDtoUi>(param);
                var c = new BookingEditClientVm(this, clientDto);                
                ClientsVms.Add(c);

            }, (x) => ClientsVms.Count < MAX_CLIENTS);
            SyncClientsCommand = new Command<object>(() =>
            {
                if (_bookingData.Clients == null)
                    _bookingData.Clients = new List<ClientDto>();
                else
                    _bookingData.Clients.Clear();

                foreach (var c in ClientsVms)
                    if (c.Model.SelectedItem != null)
                        _bookingData.Clients.Add(c.Model.SelectedItem);

            }, (x) => ClientsVms.Count > 0);
            DeleteClientCommand = new AsyncCommand<Guid>(async (param) =>
            {
                var c = ClientsVms.First(x => x.Id == param);
                _bookingData.Clients.Remove(c.Model.SelectedItem);

                await Task.FromResult(ClientsVms.Remove(c));
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
            if (!enableChecks || _bookingData.Service.NoHorseRequired)
                return;

            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            if (dto.Horses?.Any() ?? false)
            {
                horseValidationError = null;
                HorseValidationHoursPerDayWarning = null;
                HorseValidationHoursInRowWarning = null;

                if (HasDuplicates(dto.Horses))
                    horseValidationError = "Booking has duplicate horses";

                var res = await bookingsClient.HasHorsesNotOverlappedBooking(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError.Append(res.ErrorMessage);

                res = await bookingsClient.HasHorsesScheduleFitBooking(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError.Append(res.ErrorMessage);

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
