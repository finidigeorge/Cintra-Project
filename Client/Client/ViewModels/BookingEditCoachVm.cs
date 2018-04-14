using Client.Commands;
using Common;
using Common.DtoMapping;
using RestClient;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class BookingEditCoachVm : BaseVm
    {
        public Guid Id { get; private set; } = new Guid();
        private CoachesClient coachesClient = new CoachesClient();
        private readonly BookingEditWindowVm _parentVm;
        public CoachesRefVm Model { get; set; } = new CoachesRefVm();

        public bool DisplayOnlyAssignedCoaches { get; set; } = true;

        public IAsyncCommand GetCoachesCommand { get => Model.RefreshDataCommand; }
        public ICommand AddCoachCommand { get => _parentVm.AddCoachCommand; }
        public ICommand DeleteCoachCommand { get; set; }

        public BookingEditCoachVm(BookingEditWindowVm parentVm, CoachDtoUi coach)
        {
            _parentVm = parentVm;
            Model.RefreshDataCommand.Execute(null);
            Model.SelectedItem = coach;

            Model.OnSelectedItemChanged += async (sender, c) =>
            {                
                _parentVm.SyncCoachesCommand.Execute(null);
                await _parentVm.RunCoachValidations();
            };

            Model.GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                if (_parentVm?.BookingData?.Service == null)
                {
                    Model.Items?.Clear();
                    return;
                }

                long selectedItemId = 0;
                if (Model.SelectedItem != null)
                    selectedItemId = Model.SelectedItem.Id;

                if (Model.Items == null)
                    Model.Items = new ObservableCollection<CoachDtoUi>();
                else
                    Model.Items.Clear();

                if (_parentVm.BookingData.Service != null)
                    foreach (var item in (await coachesClient.GetAllByService(_parentVm.BookingData.Service.Id, DisplayOnlyAssignedCoaches)).ToList<CoachDto, CoachDtoUi>())
                        Model.Items.Add(item);

                if (selectedItemId != 0)
                {
                    Model.SelectedItem = Model.Items.FirstOrDefault(i => i.Id == selectedItemId);
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
    }

}
