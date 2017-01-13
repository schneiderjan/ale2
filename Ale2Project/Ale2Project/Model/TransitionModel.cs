using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ale2Project.Model
{
    public class TransitionModel
    {
        public StateModel BeginState { get; set; }
        public StateModel EndState { get; set; }
        public string Value { get; set; }
        public string LeftStackValue { get; set; }
        public string RightStackValue { get; set; }
    }
}
