﻿using System;
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

            var dfa = new AutomatonModel();
            var stackHistory = new List<IntermediateNfaStateModel>();
            FindNewStates(stack, stackHistory, stateToEpsilonN, ndfa, dfa);
            stackHistory = stackHistory.DistinctBy(s => s.Name).ToList();
            FindNewTransitions(stackHistory, ndfa, dfa, stateToEpsilonN);

            dfa.Alphabet = ndfa.Alphabet;
            return dfa;
        }

        private void FindNewTransitions(List<IntermediateNfaStateModel> stackHistory, AutomatonModel ndfa, AutomatonModel dfa, Dictionary<StateModel, List<StateModel>> stateToEpsilonN)
        {
            //check each transition in original automaton for each letter
            //beginstate must be in stackhistory and endstate is E*
            //then the transition is beginstate = stackhistory combined and end state is E*
            //there must be verified if the transition in the new automaton already exists
            foreach (var history in stackHistory)
            {
                foreach (var historyState in history.States)
                {
                    foreach (var transition in ndfa.Transitions)
                    {
                        foreach (var letter in ndfa.Alphabet)
                        {
                            if (historyState == transition.BeginState &&
                                transition.Value == letter.ToString())
                            {
                                var epsilonStates = stateToEpsilonN[transition.EndState];
                                if (history.States.Count == 1)
                                {
                                    if (historyState.IsInitial ||
                                        historyState.IsFinal)
                                    {
                                        var endStateName = "";
                                        foreach (var epsilonState in epsilonStates)
                                        {
                                            if (epsilonState == epsilonStates[epsilonStates.Count - 1]) endStateName += epsilonState.Name;
                                            else endStateName += epsilonState.Name + ",";
                                        }
                                        var endState = new StateModel { Name = endStateName };

                                        var trans = new TransitionModel
                                        {
                                            BeginState = historyState,
                                            EndState = endState,
                                            Value = letter.ToString()
                                        };

                                        if (dfa.States.All(s => s.Name != historyState.Name)) dfa.States.Add(historyState);
                                        if (dfa.States.All(s => s.Name != endState.Name)) dfa.States.Add(endState);
                                        dfa.Transitions.Add(trans);
                                    }
                                }
                                else if (history.States.Count > 1)
                                {
                                    var beginStateName = "";
                                    foreach (var state in history.States)
                                    {
                                        if (state == history.States[history.States.Count - 1]) beginStateName += state.Name;
                                        else beginStateName += state.Name + ",";
                                    }
                                    var beginState = new StateModel { Name = beginStateName };

                                    var endStateName = "";
                                    foreach (var epsilonState in epsilonStates)
                                    {
                                        if (epsilonState == epsilonStates[epsilonStates.Count - 1]) endStateName += epsilonState.Name;
                                        else endStateName += epsilonState.Name + ",";
                                    }
                                    var endState = new StateModel { Name = endStateName };

                                    var trans = new TransitionModel
                                    {
                                        BeginState = beginState,
                                        EndState = endState,
                                        Value = letter.ToString()
                                    };
                                    if (dfa.States.All(s => s.Name != endState.Name)) dfa.States.Add(endState);
                                    if (dfa.States.All(s => s.Name != beginState.Name)) dfa.States.Add(beginState);
                                    dfa.Transitions.Add(trans);
                                }
                            }
                        }
                    }
                }
            }

        }

        private void FindNewStates(Stack<List<StateModel>> stack, List<IntermediateNfaStateModel> stackHistory,
            Dictionary<StateModel, List<StateModel>> stateToEpsilonN, AutomatonModel ndfa, AutomatonModel nfa)
        {
            List<StateModel> currentStates;
            if (stack.Any())
            {
                currentStates = stack.Pop();
            }
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
                                AddToStackHistory(stackHistory, currentStates);
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
                                    stackHistory.Add(new IntermediateNfaStateModel()
                                    {
                                        Name = transition.EndState.Name,
                                        States = new List<StateModel> { transition.EndState }
                                    });
                                }

                                if (nfa.States.All(s => s.Name != newState.Name))
                                {
                                    nfa.States.Add(newState);
                                    stack.Push(newStatesForRecursion);
                                }
                                FindNewStates(stack, stackHistory, stateToEpsilonN, ndfa, nfa);

                                //dfa.Transitions.Add(new TransitionModel
                                //{
                                //    BeginState = state,
                                //    EndState = newState,
                                //    Value = letter.ToString()
                                //});
                            }
                        }
                    }
                }
            }
        }

        private void AddToStackHistory(List<IntermediateNfaStateModel> stackHistory, List<StateModel> currentStates)
        {
            var name = "";
            foreach (var currentState in currentStates)
            {
                name += currentState == currentStates[currentStates.Count - 1]
                    ? currentState.Name
                    : currentState.Name + ",";
            }

            stackHistory.Add(new IntermediateNfaStateModel
            {
                Name = name,
                States = currentStates
            });

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