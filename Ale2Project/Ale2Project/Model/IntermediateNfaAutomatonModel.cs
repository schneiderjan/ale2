using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ale2Project.Model
{
    public class IntermediateNfaAutomatonModel
    {
        public List<IntermediateNfaStateModel> States { get; set; }
        public List<TransitionModel> Transitions { get; set; }

        public IntermediateNfaAutomatonModel()
        {
            States = new List<IntermediateNfaStateModel>();
            Transitions = new List<TransitionModel>();
        }
    }
}
