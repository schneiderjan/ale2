using System.Collections.Generic;

namespace Ale2Project.Model
{
    public class AutomatonModel
    {
        public List<StateModel> States { get; set; }
        public List<char> Alphabet { get; set; }
        public List<TransitionModel> Transitions { get; set; }
        public bool IsNda { get; set; }

        public AutomatonModel()
        {
            States = new List<StateModel>();
            Alphabet = new List<char>();
            Transitions= new List<TransitionModel>();
        }
    }
}