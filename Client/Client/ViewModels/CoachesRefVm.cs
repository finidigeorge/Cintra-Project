using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public CoachesRefVm()
        {
            Client = RestClientFactory.GetClient<CoachDto>();
            DisplayEditItemScheduleCommand = new Command<object>(() =>
            {
                new ScheduleEditor()
                {
                    DataContext = new SchedulesRefVm()
                    {
                        DataSource = SelectedItem.Schedules
                    }
                }.ShowDialog();

            }, (x) => CanEditSelectedItem);
        }

        public ICommand DisplayEditItemScheduleCommand { get; set; }
    }

}
