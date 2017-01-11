using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var nfa = new AutomatonModel();

            //Find epsilon transitions for each state
            var stateToEpsilonN = new Dictionary<StateModel, List<StateModel>>();
            foreach (var state in ndfa.States)
            {
                FindEpsilonN(ndfa, stateToEpsilonN, state);
            }

            nfa = BuildNfa(ndfa, stateToEpsilonN);

            //create new state
            //create transition to that state
            //add to nfa




            //start at intial state and find all transitions end states of each letter in the alphabet
            //foreach end state check stateToEpsilonN E* column for new state.
            //and 

            return nfa;
        }

        private AutomatonModel BuildNfa(AutomatonModel ndfa, Dictionary<StateModel, List<StateModel>> stateToEpsilonN)
        {
            var initialState = ndfa.States.FirstOrDefault(x => x.IsInitial);
            if (initialState == null) throw new Exception("No initial state found.");
            var stateStack = new Stack<StateModel>();
            stateStack.Push(initialState);

            var nfa = new AutomatonModel();
            FindNewTransitions(initialState, stateStack, stateToEpsilonN, ndfa, nfa);
            return nfa;
        }

        private void FindNewTransitions(StateModel currentState, Stack<StateModel> stateStack, Dictionary<StateModel, List<StateModel>> stateToEpsilonN, AutomatonModel ndfa, AutomatonModel nfa)
        {
            if (!stateStack.Any()) return;

            foreach (var state in ndfa.States)
            {
                foreach (var letter in ndfa.Alphabet)
                {
                    foreach (var transition in ndfa.Transitions)
                    {
                        //check if state is in epsilon transition
                        if (currentState == state &&
                            state == transition.BeginState &&
                            letter.ToString() == transition.Value)
                        {
                            var epsilonStates = stateToEpsilonN[state];
                            var newState = new StateModel();
                            foreach (var epsilonState in epsilonStates)
                            {
                                newState.Name += epsilonState.Name;
                            }

                            if (!nfa.States.Contains(newState))
                            {
                                stateStack.Push(newState);
                            }
                           
                            //TODO: Verify if transition is good
                            nfa.Transitions.Add(new TransitionModel
                            {
                                BeginState = currentState,
                                EndState = newState,
                                Value = letter.ToString()
                            });
                            
                            if (stateStack.Any()) FindNewTransitions(stateStack.Pop(), stateStack, stateToEpsilonN, ndfa, nfa);
                        }
                    }
                }
            }
        }

        private void FindEpsilonN(AutomatonModel ndfa, Dictionary<StateModel, List<StateModel>> stateToEpsilonN, StateModel state)
        {
            foreach (var transition in ndfa.Transitions)
            {
                if (transition.BeginState == state && transition.Value == "ε")
                {
                    if (stateToEpsilonN.ContainsKey(state)) stateToEpsilonN[state].Add(transition.EndState);
                    else stateToEpsilonN.Add(state, new List<StateModel> { state, transition.EndState });

                    FindEpsilonN(ndfa, stateToEpsilonN, transition.EndState);
                }
            }
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