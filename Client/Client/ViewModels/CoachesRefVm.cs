using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Client.Windows;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;

namespace Client.ViewModels
{
    public class CoachesRefVm : BaseReferenceVm<CoachDto, CoachDtoUi>
    {
        public ICommand DisplayEditItemScheduleCommand { get; set; }

        public CoachesRefVm()
        {
            Client = RestClientFactory.GetClient<CoachDto>();
            DisplayEditItemScheduleCommand = new Command<object>(ShowScheduleEditor, (x) => CanEditSelectedItem);
        }

        private void ShowScheduleEditor()
        {
            var editor = new ScheduleEditor()
            {
                Owner = Application.Current.MainWindow,
            };

            editor.Model.Coach = SelectedItem;
            editor.Model.DataSource = SelectedItem.Schedules;
            editor.Model.RefreshDataCommand.ExecuteAsync(null);

            editor.ShowDialog();
        }        
    }

}
