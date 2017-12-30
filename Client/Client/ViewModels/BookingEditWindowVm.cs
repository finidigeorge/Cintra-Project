using Common.DtoMapping;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class BookingEditWindowVm: BaseVm
    {
        private BookingDtoUi _bookingData;
        public BookingDtoUi BookingData {
            get => _bookingData;
            set
            {
                Set(ref _bookingData, value, nameof(BookingData));
                RefreshAllModels();                               
            }
        }

        public ICommand GetClientsCommand { get => ClientsModel.RefreshDataCommand; }
        public ICommand GetServicesCommand { get=> ServicesModel.RefreshDataCommand; }
        public ICommand GetHorsesCommand { get => HorsesModel.RefreshDataCommand; }
        public ICommand GetCoachesCommand { get => CoachesModel.RefreshDataCommand; }


        public ClientsRefVm ClientsModel { get; set; } = new ClientsRefVm();
        public ServicesRefVm ServicesModel { get; set; } = new ServicesRefVm();
        public HorsesRefVm HorsesModel { get; set; } = new HorsesRefVm();
        public CoachesRefVm CoachesModel { get; set; } = new CoachesRefVm();
        public PaymentTypesRefVm PaymentTypesModel { get; set; } = new PaymentTypesRefVm();


        public BookingEditWindowVm()
        {
            ClientsModel.OnSelectedItemChanged += (sender, client) => { _bookingData.Client = client; };
            ServicesModel.OnSelectedItemChanged += async (sender, service) => {
                _bookingData.Service = service;
                await HorsesModel.RefreshDataCommand.ExecuteAsync(null);
                await CoachesModel.RefreshDataCommand.ExecuteAsync(null);
                SyncServiceDataModels();
            };
            HorsesModel.OnSelectedItemChanged += (sender, horse) => { _bookingData.Horse = horse; };
            CoachesModel.OnSelectedItemChanged += (sender, coach) => { _bookingData.Coach = coach; };
            PaymentTypesModel.OnSelectedItemChanged += (sender, paymentType) => { _bookingData.BookingPayment.PaymentType = paymentType; };
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
