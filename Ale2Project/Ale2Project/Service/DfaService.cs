using System;
using System.Collections.Generic;
using System.Linq;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class DfaService : IDfaService
    {
        public bool IsAutomatonDfa(AutomatonModel automaton)
        {
            //conditions:
            //outputs of all states are equal to alphabet count
            //outputs of all states have all alphabet values
            //only one final state
            if (!HasOneFinalState(automaton)) return false;
            if (!HasOutputsEqualAlphabetCount(automaton)) return false;
            if (!StatesHaveOutputsWithAllLetters(automaton)) return false;

            return true;
        }

        public AutomatonModel ConvertNdfaToNfa(AutomatonModel ndfa)
        {
            var stateToEpsilonN = new Dictionary<StateModel, List<StateModel>>();

            foreach (var state in ndfa.States)
            {
                foreach (var transition in ndfa.Transitions)
                {
                    if (transition.BeginState == state && transition.Value == "ε")
                    {
                        if (stateToEpsilonN.ContainsKey(state)) stateToEpsilonN[state].Add(transition.EndState);
                        else stateToEpsilonN.Add(state, new List<StateModel> { state, transition.EndState });

                        //Todo: recursively check for endstate if transition has epsilon
                    }
                }
            }


            //TODO: cHCNAGE
            return ndfa;
        }

        private bool HasOneFinalState(AutomatonModel automaton)
        {
            var finalStatesCount = automaton.States.Count(i => i.IsFinal);
            if (finalStatesCount > 1) return false;
            return true;
        }

        private bool HasOutputsEqualAlphabetCount(AutomatonModel automaton)
        {
            foreach (var automatonState in automaton.States)
            {
                int outputCount = 0;
                foreach (var automatonTransition in automaton.Transitions)
                {
                    if (automatonTransition.BeginState == automatonState) outputCount++;
                }
                if (outputCount != automaton.Alphabet.Count) return false;
            }
            return true;
        }

        private bool StatesHaveOutputsWithAllLetters(AutomatonModel automaton)
        {
            foreach (var automatonState in automaton.States)
            {
                List<char> letters = new List<char>();
                foreach (var automatonTransition in automaton.Transitions)
                {
                    if (automatonTransition.BeginState == automatonState)
                    {
                        letters.Add(Convert.ToChar(automatonTransition.Value));
                    }
                }
                var sequenceEqual = letters.OrderBy(t => t).SequenceEqual(automaton.Alphabet.OrderBy(t => t));
                if (!sequenceEqual) return false;
            }
            return true;
        }
    }
}