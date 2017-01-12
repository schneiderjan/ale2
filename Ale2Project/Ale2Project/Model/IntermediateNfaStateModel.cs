using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ale2Project.Model
{
    public class IntermediateNfaStateModel
    {
        public IntermediateNfaStateModel()
        {
            States = new List<StateModel>();
        }

        public string Name { get; set; }
        public List<StateModel> States { get; set; }
    }
}
