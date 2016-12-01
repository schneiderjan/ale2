using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class AcceptedStringCheckService : IAcceptedStringCheckService
    {
        public bool IsAcceptedString(string input, AutomatonModel automaton)
        {
            List<StateModel> currentStates = automaton.States.Where(x => x.IsInitial).ToList();
            List<StateModel> nextStates = new List<StateModel>();
            List<char> values = new List<char>();
            List<TransitionModel> possibleTransitions = new List<TransitionModel>();
            
            foreach (var c in input)
            {
                values.Add(c);
            }

            for (int i = 0; i < values.Count; i++)
            {
                var value = values[i];
                possibleTransitions = automaton.Transitions.Where(s => currentStates.Contains(s.BeginState)).ToList();
                for (int index = 0; index < possibleTransitions.Count; index++)
                {
                    var possibleTransition = possibleTransitions[index];
                    //check if transition is possible and if its the last one
                    if (possibleTransition.Value == value.ToString())
                    {
                        nextStates.Add(possibleTransition.EndState);
                    }
                    else if (possibleTransition.Value == "ε")
                    {
                        //check if epsilon endState has a transition with value[index]
                        //if yes add to nextstates
                        var transitionsAfterEpsilon =
                            automaton.Transitions.Where(
                                transition =>
                                    transition.BeginState == possibleTransition.EndState &&
                                    transition.Value == Convert.ToString(values[i]))
                                    .ToList();

                        if (transitionsAfterEpsilon.Any())
                            foreach (var transition in transitionsAfterEpsilon) nextStates.Add(transition.BeginState);
                    }

                    if (possibleTransition.EndState.IsFinal && 
                        i == values.Count - 1 &&
                        nextStates.Any()) return true;
                }
                //if yes then check the next state;
                if (!nextStates.Any()) return false;

                currentStates.Clear();
                currentStates = new List<StateModel>(nextStates);
                nextStates.Clear();
            }
            return false;
        }
    }
}
