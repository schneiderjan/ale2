using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class LanguageCheckService : ILanguageCheckService
    {
        private List<string> _words;
        private bool _isAcceptedStringFound;


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

        public List<string> GetAllWords(AutomatonModel automaton)
        {
            _words = new List<string>();

            var initialState = automaton.States.FirstOrDefault(x => x.IsInitial);
            _words = FindWords(automaton, new List<StateModel>(), initialState, "");

            return _words;
        }

        private List<string> FindWords(AutomatonModel automaton, List<StateModel> passedStates, StateModel state,
            string word)
        {
            foreach (var transition in automaton.Transitions)
            {
                if (transition.BeginState == state)
                {
                    var newWord = GetNewWord(word, transition);
                    var newPassedStates = new List<StateModel>(passedStates) { state };

                    if (!IsLoop(passedStates, state))
                    {
                        if (transition.EndState.IsFinal)
                        {
                            _words.Add(newWord);
                            TryAddTransitionOnItself(automaton, transition, newWord);
                        }
                        else
                        {
                            FindWords(automaton, newPassedStates, transition.EndState, newWord);
                        }
                    }
                    else
                    {
                        if (transition.EndState.IsFinal)
                        {
                            _words.Add(newWord);
                            TryAddTransitionOnItself(automaton, transition, newWord);
                        }
                    }
                }
            }
            return _words;
        }

        private string GetNewWord(string word, TransitionModel transition)
        {
            var newWord = "";
            if (transition.Value != "ε") newWord = word + transition.Value;
            else newWord = word;
            return newWord;
        }

        private void TryAddTransitionOnItself(AutomatonModel automaton, TransitionModel transition, string newWord)
        {
            var moreTransitions = automaton.Transitions.Where(
                x =>
                    x.BeginState == x.EndState &&
                    x.BeginState == transition.EndState);
            if (moreTransitions.Count() > 0)
            {
                foreach (var moreTransition in moreTransitions)
                {
                    _words.Add(GetNewWord(newWord, moreTransition));
                }
            }
        }

        private bool IsLoop(List<StateModel> passedStates, StateModel currentState)
        {
            var count = passedStates.Count(s => s.Name == currentState.Name);
            if (count <= 1) return false;
            return true;
        }

        public bool IsAcceptedStringByPda(string input, AutomatonModel automaton)
        {
            Stack<string> stack = new Stack<string>();
            List<char> values = new List<char>();
            List<TransitionModel> possibleTransitions = new List<TransitionModel>();
            StateModel currentState = new StateModel();
            _isAcceptedStringFound = false;

            currentState = automaton.States.FirstOrDefault(s => s.IsInitial);


            if (string.IsNullOrEmpty(input) ||
                string.IsNullOrWhiteSpace(input))
            {
                return IsEmptyWordAccepted(automaton, currentState, stack, false);
            }

            foreach (var c in input)
            {
                values.Add(c);
            }
            return TraversePda(automaton, values, currentState, stack, false);
        }



        private bool TraversePda(AutomatonModel automaton, List<char> values, StateModel currentState, Stack<string> stack, bool localIsAcceptedStringFound)
        {
            string empty = "ε";
            List<TransitionModel> possibleTransitions;

            //if there is stack but empty transition can remove stack we still have
            //to check the top of stack and see if transition can remove it
            //then another recursion, there are possibly more stack items
            if (stack.Any() &&
                !values.Any())
            {
                possibleTransitions = automaton.Transitions.Where(s => s.BeginState == currentState && s.Value == empty).ToList();
                if (!possibleTransitions.Any())
                {
                    return false;
                }

                foreach (var possibleTransition in possibleTransitions)
                {
                    if (possibleTransition.PopStack != empty &&
                        possibleTransition.PushStack == empty)
                    {
                        if (stack.Any() &&
                            stack.Peek() == possibleTransition.PopStack)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            return false;
                        }


                        if (!stack.Any() &&
                            possibleTransition.EndState.IsFinal)
                        {
                            _isAcceptedStringFound = true;
                        }

                        TraversePda(automaton, values, possibleTransition.EndState, stack, false);
                    }
                }
            }

            //check if values empty = processed all inputs
            //+ if the currentstate is an endstate
            //+ stack is empty
            if (!values.Any() &&
                currentState.IsFinal &&
                !stack.Any())
            {
                _isAcceptedStringFound = true;
            }

            foreach (var value in values)
            {
                possibleTransitions = automaton.Transitions.Where(s => s.BeginState == currentState).ToList();
                foreach (var possibleTransition in possibleTransitions)
                {
                    //must conisder also emtpyt
                    //must check if final state when word complete
                    //accepted string: final state & stack empty
                    if (possibleTransition.Value == value.ToString())
                    {
                        //pop/push = empty and stack empty 
                        if (possibleTransition.PopStack == empty &&
                            possibleTransition.PushStack == empty &&
                            !stack.Any())
                        {
                            return _isAcceptedStringFound;
                        }
                        //[_, x] case 1
                        else if (possibleTransition.PopStack == empty &&
                                 possibleTransition.PushStack != empty)
                        {
                            if (_isAcceptedStringFound)
                            {
                                return true;
                            }

                            stack.Push(possibleTransition.PushStack);
                            //var newStack = GetNewPushedStack(stack, possibleTransition.PushStack);
                            var newValues = GetNewValues(values);
                            TraversePda(automaton, newValues, possibleTransition.EndState, stack, false);
                        }
                        //[x,_] case 1
                        else if (possibleTransition.PopStack != empty &&
                                 possibleTransition.PushStack == empty)
                        {
                            if (_isAcceptedStringFound)
                            {
                                return true;
                            }

                            if (stack.Any() &&
                                stack.Peek() == possibleTransition.PopStack)
                            {
                                stack.Pop();
                            }
                            else
                            {
                                return false;
                            }

                            var newValues = GetNewValues(values);
                            TraversePda(automaton, newValues, possibleTransition.EndState, stack, false);
                        }
                        //[x,x]
                        if (possibleTransition.PopStack == possibleTransition.PushStack &&
                            possibleTransition.PopStack != empty &&
                            possibleTransition.PushStack != empty &&
                            stack.Any())
                        {
                            if (_isAcceptedStringFound)
                            {
                                return true;
                            }

                            var newValues = GetNewValues(values);
                            TraversePda(automaton, newValues, possibleTransition.EndState, stack, false);
                        }
                    }
                    else if (possibleTransition.Value == empty)
                    {
                        if (_isAcceptedStringFound)
                        {
                            return true;
                        }

                        // case 3
                        if (stack.Any())
                        {
                            if (stack.Peek() == possibleTransition.PopStack)
                            {
                                stack.Pop();
                            }
                            else if (possibleTransition.PopStack == empty) // pop stack is empty = traverse w/o removing values
                            {
                                //do nothing
                            }
                            else
                            {
                                return false;
                            }
                            TraversePda(automaton, values, possibleTransition.EndState, stack, false);
                        }
                        //case 4
                        else
                        {
                            if (possibleTransition.PopStack == empty &&
                                possibleTransition.PushStack == empty)
                            {
                                TraversePda(automaton, values, possibleTransition.EndState, stack, false);
                            }
                            else if (possibleTransition.PopStack == empty &&
                                     possibleTransition.PushStack != empty)
                            {
                                stack.Push(possibleTransition.PushStack);
                                TraversePda(automaton, values, possibleTransition.EndState, stack, false);
                            }
                        }
                    }
                }


                //  if (!stack.Any() && value == values[values.Count - 1]) return true; //doesnt consider end state

            }

            return _isAcceptedStringFound;
        }

        private bool IsEmptyWordAccepted(AutomatonModel automaton, StateModel currentState, Stack<string> stack, bool localIsAcceptedStringFound)
        {

            string empty = "ε";
            List<TransitionModel> possibleTransitions;
            if (currentState.IsFinal &&
                !stack.Any())
            {
                //check if final is start and see if transition exists
                //where push/pop = empty
                //else false
                if (currentState.IsFinal &&
                    currentState.IsInitial)
                {
                    possibleTransitions = automaton.Transitions.Where(s =>
                    s.BeginState == currentState &&
                    s.BeginState == s.EndState &&
                    s.PopStack == empty &&
                    s.PushStack == empty)
                    .ToList();

                    if (possibleTransitions.Any())
                    {
                        _isAcceptedStringFound = true;
                    }
                    else
                    {
                        return false;
                    }

                }

                _isAcceptedStringFound = true;
            }


            possibleTransitions = automaton.Transitions.Where(s => s.BeginState == currentState).ToList();
            foreach (var possibleTransition in possibleTransitions)
            {
                if (possibleTransition.Value == empty)
                {
                    if (possibleTransition.PopStack == empty &&
                     possibleTransition.PushStack == empty &&
                     !stack.Any())
                    {
                        if (possibleTransition.EndState.IsFinal)
                        {
                            _isAcceptedStringFound = true;
                        }
                        else
                        {
                            IsEmptyWordAccepted(automaton, possibleTransition.EndState, stack, false);
                        }
                    }
                    else if (possibleTransition.PopStack == empty &&
                             possibleTransition.PushStack != empty)
                    {
                        if (_isAcceptedStringFound)
                        {
                            return true;
                        }

                        stack.Push(possibleTransition.PushStack);
                        IsEmptyWordAccepted(automaton, possibleTransition.EndState, stack, false);
                    }
                    //[x,_] case 1
                    else if (possibleTransition.PopStack != empty &&
                             possibleTransition.PushStack == empty)
                    {
                        if (_isAcceptedStringFound)
                        {
                            return true;
                        }

                        if (stack.Any() &&
                            stack.Peek() == possibleTransition.PopStack)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            return false;
                        }

                        IsEmptyWordAccepted(automaton, possibleTransition.EndState, stack, false);
                    }
                }
            }


            return _isAcceptedStringFound;
        }

        private List<char> GetNewValues(List<char> values)
        {
            List<char> newValues = new List<char>(values);
            newValues.RemoveAt(0);
            return newValues;
        }

        private Stack<string> GetNewPoppedStack(Stack<string> stack)
        {
            Stack<string> newStack = new Stack<string>(stack);
            newStack.Pop();
            return newStack;
        }

        private Stack<string> GetNewPushedStack(Stack<string> stack, string pushValue)
        {
            Stack<string> newStack = new Stack<string>(stack);
            newStack.Push(pushValue);
            return newStack;
        }
    }
}
