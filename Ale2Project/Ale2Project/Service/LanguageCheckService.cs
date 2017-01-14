using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class LanguageCheckService : ILanguageCheckService
    {
        private List<string> _words;

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

        private List<string> FindWords(AutomatonModel automaton, List<StateModel> passedStates, StateModel state, string word)
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
                        else FindWords(automaton, newPassedStates, transition.EndState, newWord);
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
            List<StateModel> currentStates = new List<StateModel>();
            List<TransitionModel> possibleTransitions = new List<TransitionModel>();


            foreach (var c in input)
            {
                values.Add(c);
            }

            currentStates = automaton.States.Where(s => s.IsInitial).ToList();

            foreach (var value in values)
            {
                possibleTransitions = automaton.Transitions.Where(s => currentStates.Contains(s.BeginState)).ToList();
                foreach (var possibleTransition in possibleTransitions)
                {
                    
                }
            }


            return true;
        }
    }
}
