using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
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
            //FindEpsilonN only adds the E* which have an actual E transitions
            //but does not consider the state itself then, hence the loop below
            foreach (var state in ndfa.States)
            {
                if (!stateToEpsilonN.ContainsKey(state))
                {
                    stateToEpsilonN.Add(state, new List<StateModel> { state });
                }
            }

            nfa = BuildNfa(ndfa, stateToEpsilonN);
            return nfa;
        }

        private AutomatonModel BuildNfa(AutomatonModel ndfa, Dictionary<StateModel, List<StateModel>> stateToEpsilonN)
        {
            var initialState = ndfa.States.FirstOrDefault(x => x.IsInitial);
            if (initialState == null) throw new Exception("No initial state found.");
            var initList = new List<StateModel> { initialState };
            var stack = new Stack<List<StateModel>>();
            stack.Push(initList);

            var interNfa = new IntermediateNfaAutomatonModel();
            FindNewStates(stack, stateToEpsilonN, ndfa, interNfa);
           //TODO: make FindNewTransitions
            var nfa = new AutomatonModel();
            nfa.Alphabet = ndfa.Alphabet;


            return nfa;
        }

        private void FindNewStates(Stack<List<StateModel>> stack,
            Dictionary<StateModel, List<StateModel>> stateToEpsilonN, AutomatonModel ndfa, IntermediateNfaAutomatonModel nfa)
        {
            List<StateModel> currentStates;
            if (stack.Any()) currentStates = stack.Pop();
            else return;

            foreach (var currentState in currentStates)
            {
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
                                var epsilonStates = stateToEpsilonN[transition.EndState];
                                var newStatesForRecursion = new List<StateModel>();
                                var newState = new StateModel();

                                foreach (var epsilonState in epsilonStates)
                                {
                                    newStatesForRecursion.Add(epsilonState);
                                    newState.Name += epsilonState.Name + ",";
                                }
                                if (newState.Name.EndsWith(",")) newState.Name = newState.Name.TrimEnd(',');

                                //TODO: eventually add something extra for final state
                                if ((state.IsInitial || state.IsFinal) &&
                                    nfa.States.All(s => s.Name != state.Name))
                                {
                                    nfa.States.Add(state);
                                }
                                else if (transition.EndState.IsFinal &&
                                         nfa.States.All(s => s.Name != transition.EndState.Name))
                                {
                                    nfa.States.Add(transition.EndState);
                                }
                                stack.Push(newStatesForRecursion);
                                if (nfa.States.All(s => s.Name != newState.Name))
                                {
                                    nfa.States.Add(newState);
                                }
                            }
                        }
                    }
                }
            }
            FindNewStates(stack, stateToEpsilonN, ndfa, nfa);
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