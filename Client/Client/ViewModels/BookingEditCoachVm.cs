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
    }

}
