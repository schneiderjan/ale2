using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ale2Project.Model
{
    public class IntermediateDfaStateModel
    {
        public IntermediateDfaStateModel()
        {
            States = new List<StateModel>();
        }

        public string Name { get; set; }
        public List<StateModel> States { get; set; }
        public List<string> alphabetFlags { get; set; }
        public StateModel OriginatingState { get; set; }
        public string Value { get; set; }
    }
}
