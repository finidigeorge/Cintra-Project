﻿using Common.DtoMapping;
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
            ServicesModel.OnSelectedItemChanged += (sender, service) => { _bookingData.Service = service; };
            HorsesModel.OnSelectedItemChanged += (sender, horse) => { _bookingData.Horse = horse; };
            CoachesModel.OnSelectedItemChanged += (sender, coach) => { _bookingData.Coach = coach; };
            PaymentTypesModel.OnSelectedItemChanged += (sender, paymentType) => { _bookingData.BookingPayment.PaymentType = paymentType; };
        }

        private async Task RefreshAllModels()
        {
            await ClientsModel.RefreshDataCommand.ExecuteAsync(null);
            await ServicesModel.RefreshDataCommand.ExecuteAsync(null);
            await HorsesModel.RefreshDataCommand.ExecuteAsync(null);
            await CoachesModel.RefreshDataCommand.ExecuteAsync(null);
            await PaymentTypesModel.RefreshDataCommand.ExecuteAsync(null);
            SyncDataToModels();
        }

        private void SyncDataToModels()
        {
            if (BookingData.Client != null)
            {
                ClientsModel.SelectedItem = ClientsModel.Items.FirstOrDefault(x => x.Id == BookingData.Client.Id);
            }

            if (BookingData.Service != null)
            {
                ServicesModel.SelectedItem = ServicesModel.Items.FirstOrDefault(x => x.Id == BookingData.Service.Id);
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
