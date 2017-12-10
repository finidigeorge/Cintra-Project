using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using Client.Windows;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class ServicesRefVm : BaseReferenceVm<ServiceDto, ServiceDtoUi>
    {
        public ICommand DisplayEditServiceCommand { get; set; }

        public ServicesRefVm()
        {
            Client = RestClientFactory.GetClient<ServiceDto>();
            DisplayEditServiceCommand = new Command<object>(ShowServiceEditor, (x) => CanEditSelectedItem);
        }

        private void ShowServiceEditor()
        {
            var editor = new ServiceEditWindow()
            {
                Owner = Application.Current.MainWindow,
            };

            editor.Model.ServiceData = SelectedItem;
            if ((bool)editor.ShowDialog())
            {
                SelectedItem.Coaches.Clear();
                SelectedItem.Coaches.AddRange(editor.Model.GetSelectedCoached());

                SelectedItem.Horses.Clear();
                SelectedItem.Horses.AddRange(editor.Model.GetSelectedHorses());

                UpdateSelectedItemCommand.Execute(SelectedItem);
            }
        }
    }

}
