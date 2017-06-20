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
        private string _empty = "ε";

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

        public AutomatonModel ConvertNdfaToDfa(AutomatonModel ndfa)
        {
            var dfa = new AutomatonModel();
            var newStates = new Dictionary<StateModel, List<StateModel>>();
            List<StateModel> stateHistory = new List<StateModel>();
            Queue<IntermediateDfaStateModel> stack = new Queue<IntermediateDfaStateModel>();
            StateModel currentState = new StateModel();

            var initialState = ndfa.States.FirstOrDefault(x => x.IsInitial);
            if (initialState == null)
            {
                throw new Exception("No initial state found.");
            }
            stateHistory.Add(initialState);

            stack.Enqueue(new IntermediateDfaStateModel
            {
                Name = initialState.Name,
                States = new List<StateModel> { initialState }
            });
            dfa.States.Add(initialState);
            dfa.Alphabet = ndfa.Alphabet;

            TraverseNfa(stack, stateHistory, ndfa, dfa);

            return dfa;

        }

        private void TraverseNfa(Queue<IntermediateDfaStateModel> queue, List<StateModel> stateHistory, AutomatonModel ndfa, AutomatonModel dfa)
        {
            IntermediateDfaStateModel currentStates = new IntermediateDfaStateModel();
            if (queue.Any())
            {
                currentStates = queue.Dequeue();
            }
            else
            {
                return;
            }
            var originatingState = dfa.States.Find(s => s.Name == currentStates.Name);

            foreach (var letter in ndfa.Alphabet)
            {
                IntermediateDfaStateModel newHistoryItem = new IntermediateDfaStateModel();

                foreach (var currentState in currentStates.States)
                {
                    foreach (var state in ndfa.States)
                    {
                        foreach (var transition in ndfa.Transitions)
                        {
                            if (currentState == state &&
                                transition.Value == letter.ToString() &&
                                transition.BeginState == currentState)
                            {
                                //create history item
                                //add each state to states list and add statename like += to history name
                                newHistoryItem.States.Add(transition.EndState);
                                newHistoryItem.Name += transition.EndState.Name;
                                newHistoryItem.Value = transition.Value;
                                newHistoryItem.OriginatingState = originatingState;
                            }
                            else if (currentState == state &&
                                     transition.Value == _empty &&
                                     transition.BeginState == currentState)
                            {
                                IntermediateDfaStateModel epsilonHistoryItem = FindEpsilonTransitions(originatingState, currentState, transition.EndState, letter, ndfa, dfa);

                                if (epsilonHistoryItem.Name == null) continue;

                                newHistoryItem.States.AddRange(epsilonHistoryItem.States);
                                newHistoryItem.Name += epsilonHistoryItem.Name;
                                newHistoryItem.Value = epsilonHistoryItem.Value;
                                newHistoryItem.OriginatingState = originatingState;
                            }
                        }
                    }
                    //build new state and transition of the dfa
                    //also add the new state to the stack

                }
                if (newHistoryItem.Name == null) continue;

                var newState = AddHistoryToDfa(dfa, newHistoryItem);
                if (stateHistory.All(s => s.Name != newState.Name))
                {
                    queue.Enqueue(newHistoryItem);
                    stateHistory.Add(newState);
                }

            }
            //from the history add to stack
            //and also create new state with transition


            if (queue.Any())
            {
                TraverseNfa(queue, stateHistory, ndfa, dfa);
            }
            else
            {
                //check also if a final state exists
                //Im not entirely sure about the theory for dfa's
                //but i made only one final state possible.
                //either the last processed one or
                //if the first processed state was initial already
                if (dfa.States.Any(s => s.IsFinal))
                {
                    return;
                }
                var lastState = dfa.States.LastOrDefault();
                if (lastState != null)
                {
                    lastState.IsFinal = true;
                }
            }
        }

        private IntermediateDfaStateModel FindEpsilonTransitions(StateModel originatingState, StateModel currentState, StateModel transitionEndState, char letter, AutomatonModel ndfa, AutomatonModel dfa)
        {
            IntermediateDfaStateModel historyItem = new IntermediateDfaStateModel();
            foreach (var transition in ndfa.Transitions)
            {
                if (transition.BeginState == transitionEndState &&
                    transition.Value == letter.ToString())
                {
                    //create the new history item and return
                    historyItem.States.Add(transition.EndState);
                    historyItem.Name += transition.EndState.Name;
                    historyItem.Value = transition.Value;
                    historyItem.OriginatingState = originatingState;
                }
                else if (transition.BeginState == transitionEndState &&
                         transition.Value == _empty)
                {
                    //continue recursion
                }
            }
            return historyItem;
        }

        private StateModel AddHistoryToDfa(AutomatonModel dfa, IntermediateDfaStateModel newHistoryItem)
        {

            var newState = new StateModel()
            {
                Name = newHistoryItem.Name,
            };

            if (dfa.States.All(s => s.Name != newState.Name))
            {
                dfa.States.Add(newState);
            }
            else
            {
                newState = dfa.States.Find(s => s.Name == newState.Name);
            }

            dfa.Transitions.Add(new TransitionModel
            {
                BeginState = newHistoryItem.OriginatingState,
                EndState = newState,
                Value = newHistoryItem.Value
            });
            return newState;
        }

        //public AutomatonModel ConvertNdfaToDfa(AutomatonModel ndfa)
        //{
        //    var dfa = new AutomatonModel();

        //    //Find epsilon transitions for each state
        //    var stateToEpsilonN = new Dictionary<StateModel, List<StateModel>>();
        //    foreach (var state in ndfa.States)
        //    {
        //        FindEpsilonN(ndfa, stateToEpsilonN, state);
        //    }
        //    //FindEpsilonN only adds the E* which have an actual E transitions
        //    //but does not consider the state itself then, hence the loop below
        //    foreach (var state in ndfa.States)
        //    {
        //        if (!stateToEpsilonN.ContainsKey(state))
        //        {
        //            stateToEpsilonN.Add(state, new List<StateModel> { state });
        //        }
        //    }

        //    dfa = BuildDfa(ndfa, stateToEpsilonN);
        //    return dfa;
        //}

        private AutomatonModel BuildDfa(AutomatonModel ndfa, Dictionary<StateModel, List<StateModel>> stateToEpsilonN)
        {
            var initialState = ndfa.States.FirstOrDefault(x => x.IsInitial);
            if (initialState == null)
            {
                throw new Exception("No initial state found.");
            }

            var initList = new List<StateModel> { initialState };
            var stack = new Stack<List<StateModel>>();
            stack.Push(initList);

            var dfa = new AutomatonModel();
            var stackHistory = new List<IntermediateDfaStateModel>();
            FindNewStates(stack, stackHistory, stateToEpsilonN, ndfa, dfa);
            stackHistory = stackHistory.DistinctBy(s => s.Name).ToList();
            FindNewTransitions(stackHistory, ndfa, dfa, stateToEpsilonN);

            dfa.Alphabet = ndfa.Alphabet;
            return dfa;
        }

        private void FindNewTransitions(List<IntermediateDfaStateModel> stackHistory, AutomatonModel ndfa, AutomatonModel dfa, Dictionary<StateModel, List<StateModel>> stateToEpsilonN)
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

        private void FindNewStates(Stack<List<StateModel>> stack, List<IntermediateDfaStateModel> stackHistory, Dictionary<StateModel, List<StateModel>> stateToEpsilonN, AutomatonModel ndfa, AutomatonModel dfa)
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
                                    dfa.States.All(s => s.Name != state.Name))
                                {
                                    dfa.States.Add(state);

                                }
                                else if (transition.EndState.IsFinal &&
                                         dfa.States.All(s => s.Name != transition.EndState.Name))
                                {
                                    dfa.States.Add(transition.EndState);
                                    stackHistory.Add(new IntermediateDfaStateModel()
                                    {
                                        Name = transition.EndState.Name,
                                        States = new List<StateModel> { transition.EndState }
                                    });
                                }

                                if (dfa.States.All(s => s.Name != newState.Name))
                                {
                                    dfa.States.Add(newState);
                                    stack.Push(newStatesForRecursion);
                                }
                                FindNewStates(stack, stackHistory, stateToEpsilonN, ndfa, dfa);

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

        private void AddToStackHistory(List<IntermediateDfaStateModel> stackHistory, List<StateModel> currentStates)
        {
            var name = "";
            foreach (var currentState in currentStates)
            {
                name += currentState == currentStates[currentStates.Count - 1]
                    ? currentState.Name
                    : currentState.Name + ",";
            }

            stackHistory.Add(new IntermediateDfaStateModel
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
                    if (stateToEpsilonN.ContainsKey(state))
                    {
                        stateToEpsilonN[state].Add(transition.EndState);
                    }
                    else
                    {
                        stateToEpsilonN.Add(state, new List<StateModel> { state, transition.EndState });
                    }

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