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
using PropertyChanged;
using Client.Controls.WpfScheduler;

namespace Client.ViewModels
{
    public class BookingEditWindowVm: BaseVm
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
                        await RunHorseValidations();
                        await RunCoachValidations();
                    }
                };

                RefreshAllModels();                               
            }
        }

        public bool IsEditMode { get; set; }
               
        public bool ShowRecurringTab { get => !IsEditMode && (BookingData?.IsValid ?? false); }
        public bool EnableRecurringApointments { get; set; }
        public int RecurringWeeksNumber { get; set; } = 10;
        public DateTime RecurringStartDate { get; set; } = DateTime.Now.TruncateToNextWeekday(DayOfWeek.Monday);

        public ICommand AddWeeklyScheduledIntervalCommand { get; set; }
        public ICommand UpdateWeeklyScheduledIntervalCommand { get; set; }
        public ICommand DeleteWeeklyScheduledIntervalCommand { get; set; }

        public ICommand GetClientsCommand { get => ClientsModel.RefreshDataCommand; }
        public ICommand GetServicesCommand { get=> ServicesModel.RefreshDataCommand; }
        public ICommand GetHorsesCommand { get => HorsesModel.RefreshDataCommand; }
        public ICommand GetCoachesCommand { get => CoachesModel.RefreshDataCommand; }


        public Scheduler RecurrentScheduler { get; set; }

        public ClientsRefVm ClientsModel { get; set; } = new ClientsRefVm();
        public ServicesRefVm ServicesModel { get; set; } = new ServicesRefVm();
        public HorsesRefVm HorsesModel { get; set; } = new HorsesRefVm();
        public CoachesRefVm CoachesModel { get; set; } = new CoachesRefVm();
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

        public BookingEditWindowVm()
        {            
            ClientsModel.OnSelectedItemChanged += (sender, client) => {
                _bookingData.Client = client;

                OnPropertyChanged(nameof(ShowRecurringTab));
            };
            ServicesModel.OnSelectedItemChanged += async (sender, service) => {
                _bookingData.Service = service;
                await HorsesModel.RefreshDataCommand.ExecuteAsync(null);
                await CoachesModel.RefreshDataCommand.ExecuteAsync(null);

                SyncServiceDataModels();
                OnPropertyChanged(nameof(ShowRecurringTab));
            };
            HorsesModel.OnSelectedItemChanged += async (sender, horse) =>
            {
                _bookingData.Horse = horse;
                await RunHorseValidations();                
            };
            CoachesModel.OnSelectedItemChanged += async (sender, coach) =>
            {
                _bookingData.Coach = coach;
                await RunCoachValidations();
            };
            PaymentTypesModel.OnSelectedItemChanged += (sender, paymentType) => { _bookingData.BookingPayment.PaymentType = paymentType; };
        }

        private async Task RunCoachValidations()
        {
            var dto = ObjectMapper.Map<BookingDto>(_bookingData);
            coachValidationError = null;

            if (dto.Coach != null)
            {
                var res = await bookingsClient.HasCoachNotOverlappedBooking(dto);
                if (!res.Result)
                    coachValidationError = res.ErrorMessage;

                res = await bookingsClient.HasCoachScheduleFitBooking(dto);
                if (!res.Result)
                    coachValidationError = coachValidationError + (!string.IsNullOrEmpty(coachValidationError) ? ", " : "") + res.ErrorMessage;
            }

            //to raise validation checks
            _bookingData.OnPropertyChanged(nameof(_bookingData.Coach));
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
            await CoachesModel.RefreshDataCommand.ExecuteAsync(null);
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

                linkedItems = new HashSet<long>(BookingData.Service.Coaches.Select(x => x.Id));
                foreach (var c in CoachesModel.Items.Where(x => !linkedItems.Contains(x.Id)).ToList())
                    CoachesModel.Items.Remove(c);
            }

            if (BookingData.Horse != null)
            {
                HorsesModel.SelectedItem = HorsesModel.Items.FirstOrDefault(x => x.Id == BookingData.Horse.Id);
            }

            if (BookingData.Coach != null)
            {
                CoachesModel.SelectedItem = CoachesModel.Items.FirstOrDefault(x => x.Id == BookingData.Coach.Id);
            }            
        }

        private void SyncAllDataToModels()
        {
            if (BookingData.Client != null)
            {                
                ClientsModel.SelectedItem = ClientsModel.Items.FirstOrDefault(x => x.Id == BookingData.Client.Id);
            }

            if (BookingData.Service != null)
            {
                ServicesModel.SelectedItem = ServicesModel.Items.FirstOrDefault(x => x.Id == BookingData.Service.Id);

                var linkedItems = new HashSet<long>(BookingData.Service.Horses.Select(x => x.Id));
                foreach (var h in HorsesModel.Items.Where(x => !linkedItems.Contains(x.Id)).ToList())
                    HorsesModel.Items.Remove(h);

                linkedItems = new HashSet<long>(BookingData.Service.Coaches.Select(x => x.Id));
                foreach (var c in CoachesModel.Items.Where(x => !linkedItems.Contains(x.Id)).ToList())
                    CoachesModel.Items.Remove(c);
            }

            if (BookingData.Horse != null)
            {                
                HorsesModel.SelectedItem = HorsesModel.Items.FirstOrDefault(x => x.Id == BookingData.Horse.Id);
            }

            if (BookingData.Coach != null)
            {                
                CoachesModel.SelectedItem = CoachesModel.Items.FirstOrDefault(x => x.Id == BookingData.Coach.Id);
            }

            if (BookingData.BookingPayment?.PaymentType != null)
            {
                PaymentTypesModel.SelectedItem = PaymentTypesModel.Items.FirstOrDefault(x => x.Id == BookingData.BookingPayment.PaymentType.Id);
            }
        }

    }
}
