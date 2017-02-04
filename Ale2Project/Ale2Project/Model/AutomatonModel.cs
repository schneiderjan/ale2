using System.Collections.Generic;

namespace Ale2Project.Model
{
    public class AutomatonModel
    {
        public List<StateModel> States { get; set; }
        public List<char> Alphabet { get; set; }
        public List<TransitionModel> Transitions { get; set; }
        public List<string> AccecptedStack { get; set; }
        public Dictionary<string, bool> Words { get; set; }
        public bool IsDfa { get; set; }
        public bool IsPda { get; set; }
        public bool IsDfaInFile { get; set; }
        public bool IsFiniteInFile { get; set; }

        public AutomatonModel()
        {
            States = new List<StateModel>();
            Alphabet = new List<char>();
            Transitions= new List<TransitionModel>();
            AccecptedStack = new List<string>();
            Words = new Dictionary<string, bool>();
        }
    }
}