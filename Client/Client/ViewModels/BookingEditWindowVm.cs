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
    public class BookingEditCoachVm : BaseVm
    {
        public Guid Id { get; private set; } = new Guid();
        private CoachesClient coachesClient = new CoachesClient();        
        private readonly BookingEditWindowVm _parentVm;
        public CoachesRefVm CoachesModel { get; set; } = new CoachesRefVm();

        public CoachDtoUi Coach { get; set; } = new CoachDtoUi();

        public bool DisplayOnlyAssignedCoaches { get; set; } = true;

        public IAsyncCommand GetCoachesCommand { get => CoachesModel.RefreshDataCommand; }
        public ICommand AddCoachCommand { get => _parentVm.AddCoachCommand; }
        public ICommand DeleteCoachCommand { get; set; }

        public BookingEditCoachVm(BookingEditWindowVm parentVm)
        {
            _parentVm = parentVm;
            
            CoachesModel.OnSelectedItemChanged += async (sender, coach) =>
            {
                Coach = coach;
                await _parentVm.RunCoachValidations();
            };

            CoachesModel.GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                long selectedItemId = 0;
                if (CoachesModel.SelectedItem != null)
                    selectedItemId = CoachesModel.SelectedItem.Id;

                if (CoachesModel.Items == null)
                    CoachesModel.Items = new ObservableCollection<CoachDtoUi>();
                else
                    CoachesModel.Items.Clear();

                if (_parentVm.BookingData.Service != null)
                    foreach (var item in (await coachesClient.GetAllByService(_parentVm.BookingData.Service.Id, DisplayOnlyAssignedCoaches)).ToList<CoachDto, CoachDtoUi>())
                        CoachesModel.Items.Add(item);

                if (selectedItemId != 0)
                {
                    CoachesModel.SelectedItem = CoachesModel.Items.FirstOrDefault(i => i.Id == selectedItemId);
                }
            });

            DeleteCoachCommand = new AsyncCommand<object>(async (param) =>
            {
                await _parentVm.DeleteCoachCommand.ExecuteAsync(Id);
            }, (x) => _parentVm.CanDeleteCoach);

            PropertyChanged += (sender, args) => {
                if (args.PropertyName == nameof(DisplayOnlyAssignedCoaches))
                {
                    GetCoachesCommand.Execute(null);
                }
            };
        }

        public void SyncToParentVmService()
        {
            if (_parentVm.BookingData.Service != null)
            {                
                var linkedItems = new HashSet<long>(_parentVm.BookingData.Service.Coaches.Select(x => x.Id));
                foreach (var c in CoachesModel.Items.Where(x => !linkedItems.Contains(x.Id)).ToList())
                    CoachesModel.Items.Remove(c);
            }
        }
    }

    public class BookingEditWindowVm : BaseVm
    {
        BookingsClient bookingsClient = new BookingsClient();
        CoachesClient coachesClient = new CoachesClient();

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

        public ICommand GetClientsCommand { get => ClientsModel.RefreshDataCommand; }
        public ICommand GetServicesCommand { get => ServicesModel.RefreshDataCommand; }
        public ICommand GetHorsesCommand { get => HorsesModel.RefreshDataCommand; }

        public ICommand AddCoachCommand { get; set; }
        public IAsyncCommand DeleteCoachCommand { get; set; }

        public bool CanDeleteCoach { get => CoachesVms.Count > 1; }


        public Scheduler RecurrentScheduler { get; set; }

        public ClientsRefVm ClientsModel { get; set; } = new ClientsRefVm();
        public ServicesRefVm ServicesModel { get; set; } = new ServicesRefVm();
        public HorsesRefVm HorsesModel { get; set; } = new HorsesRefVm();        
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

        public BookingEditWindowVm()
        {            
            ClientsModel.OnSelectedItemChanged += (sender, client) => {
                _bookingData.Client = client;

                OnPropertyChanged(nameof(ShowRecurringTab));
            };
            ServicesModel.OnSelectedItemChanged += async (sender, service) => {
                _bookingData.Service = service;
                await HorsesModel.RefreshDataCommand.ExecuteAsync(null);

                foreach(var c in CoachesVms)
                    await c.GetCoachesCommand.ExecuteAsync(c.DisplayOnlyAssignedCoaches ? service.Id : (long?)null);

                SyncServiceDataModels();

                _bookingData.EndTime = _bookingData.BeginTime.AddMinutes((int)service.LengthMinutes);
                _bookingData.OnPropertyChanged("EndTime");
                OnPropertyChanged(nameof(ShowRecurringTab));
            };
            HorsesModel.OnSelectedItemChanged += async (sender, horse) =>
            {
                _bookingData.Horse = horse;
                await RunHorseValidations();                
            };
            
            PaymentTypesModel.OnSelectedItemChanged += (sender, paymentType) => { _bookingData.BookingPayment.PaymentType = paymentType; };
            

            AddCoachCommand = new Command<object>(() =>
            {
                CoachesVms.Add(new BookingEditCoachVm(this));
            }, (x) => CoachesVms.Count < 5);

            DeleteCoachCommand = new AsyncCommand<Guid>(async (param) =>
            {
                CoachesVms.Remove(CoachesVms.First(x => x.Id == param));                
            }, (x) => CoachesVms.Count > 1);

            AddCoachCommand.Execute(null);
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

        private async Task RunHorseValidations()
        {
            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            if (dto.Horse != null)
            {
                horseValidationError = null;
                HorseValidationHoursPerDayWarning = null;
                HorseValidationHoursInRowWarning = null;

                var res = await bookingsClient.HasHorseNotOverlappedBooking(dto);
                if (!res.Result)
                    horseValidationError = res.ErrorMessage;

                res = await bookingsClient.HasHorseScheduleFitBooking(dto);
                if (!res.Result)
                    horseValidationError = horseValidationError + (!string.IsNullOrEmpty(horseValidationError) ? ", " : "") + res.ErrorMessage;

                res = await bookingsClient.HasHorseRequiredBreak(dto);
                if (!res.Result)
                    HorseValidationHoursInRowWarning = res.ErrorMessage;

                res = await bookingsClient.HasHorseWorkedLessThanAllowed(dto);
                if (!res.Result)
                    HorseValidationHoursPerDayWarning = res.ErrorMessage;

            }

            //to raise validation checks
            _bookingData.OnPropertyChanged(nameof(_bookingData.Horse));
            OnPropertyChanged(nameof(ShowRecurringTab));
        }

        private async void RefreshAllModels()
        {
            await ClientsModel.RefreshDataCommand.ExecuteAsync(null);
            await ServicesModel.RefreshDataCommand.ExecuteAsync(null);
            await HorsesModel.RefreshDataCommand.ExecuteAsync(null);
            
            await PaymentTypesModel.RefreshDataCommand.ExecuteAsync(null);
            SyncAllDataToModels();
        }

        private void SyncServiceDataModels()
        {
            if (BookingData.Service != null)
            {                
                var linkedItems = new HashSet<long>(BookingData.Service.Horses.Select(x => x.Id));
                foreach (var h in HorsesModel.Items.Where(x => !linkedItems.Contains(x.Id)).ToList())
                    HorsesModel.Items.Remove(h);

                /*foreach (var vm in CoachesVms)
                    vm.SyncToParentVmService();*/
            }

            if (BookingData.Horse != null)
            {
                HorsesModel.SelectedItem = HorsesModel.Items.FirstOrDefault(x => x.Id == BookingData.Horse.Id);
            }                      
        }

        private void SyncAllDataToModels()
        {
            if (BookingData.Client != null)
            {                
                ClientsModel.SelectedItem = ClientsModel.Items.FirstOrDefault(x => x.Id == BookingData.Client.Id);
            }

            SyncServiceDataModels();

            if (BookingData.BookingPayment?.PaymentType != null)
            {
                PaymentTypesModel.SelectedItem = PaymentTypesModel.Items.FirstOrDefault(x => x.Id == BookingData.BookingPayment.PaymentType.Id);
            }
        }

    }
}
