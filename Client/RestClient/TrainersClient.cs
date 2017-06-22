using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;

namespace RestClient
{
    public class TrainersClient: BaseRestApiClient<TrainerDto>
    {
        public TrainersClient() : base("trainers")
        {
        }
    }
}
