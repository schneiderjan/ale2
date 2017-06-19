using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class FileService : IFileService
    {
        private string states = "states:";
        private string final = "final:";
        private string alphabet = "alphabet:";

        public GraphVizFileModel ReadFile()
        {
            GraphVizFileModel fileModel = new GraphVizFileModel();
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for fileModel extension and default fileModel extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected fileModel name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                fileModel.FilePath = filename;
                fileModel.Lines = System.IO.File.ReadLines(filename).ToList();
            }
            return fileModel;
        }
        public AutomatonModel ParseGraphVizFile(GraphVizFileModel graphVizFileModel)
        {
            string states = "states:";
            string final = "final:";
            string alphabet = "alphabet:";
            string stack = "stack:";

            var automaton = new AutomatonModel();
            foreach (var line in graphVizFileModel.Lines)
            {
                if (line.Contains(alphabet))
                {
                    var chars = GetSubstring(line, alphabet);
                    foreach (var c in chars)
                    {
                        automaton.Alphabet.Add(char.ToLower(c));
                    }
                }
                else if (line.Contains(stack))
                {
                    var chars = GetSubstring(line, stack);
                    foreach (var c in chars)
                    {
                        automaton.AccecptedStack.Add(char.ToLower(c).ToString());
                    }
                    automaton.IsPda = true;
                }
                else if (line.Contains(states))
                {
                    var sub = GetSubstring(line, states);
                    var chars = SplitOnComma(sub);
                    foreach (var c in chars)
                    {
                        var state = new StateModel() { Name = c.ToUpper() };
                        automaton.States.Add(state);
                    }
                }
                else if (line.Contains(final))
                {
                    var sub = GetSubstring(line, final);
                    var chars = SplitOnComma(sub);
                    foreach (var c in chars)
                    {
                        foreach (var state in automaton.States)
                        {
                            if (c.Equals(state.Name)) state.IsFinal = true;
                        }
                    }
                }
                else if (line.Contains("transitions"))
                {
                    var lower = graphVizFileModel.Lines.IndexOf(line) + 1;
                    //todo: change upper to next "end."
                    var upper = FindUpperLimit(graphVizFileModel.Lines.IndexOf(line), graphVizFileModel.Lines);

                    var transitions = new List<TransitionModel>();
                    transitions = !automaton.IsPda ?
                        ReadRegularAutomatonTransitions(graphVizFileModel, lower, upper, automaton) :
                        ReadPdaTransitions(graphVizFileModel, lower, upper, automaton);

                    automaton.Transitions = transitions;
                    //break;
                }
                else if (line.Contains("words"))
                {
                    var lower = graphVizFileModel.Lines.IndexOf(line) + 1;
                    var upper = FindUpperLimit(graphVizFileModel.Lines.IndexOf(line), graphVizFileModel.Lines);
                    automaton.Words = ReadWords(graphVizFileModel, lower, upper);
                }
                else if (line.Contains("dfa"))
                {
                    var last = line[line.Length - 1];
                    automaton.IsDfaInFile = last == 'y';
                }
                else if (line.Contains("finite"))
                {
                    var last = line[line.Length - 1];
                    automaton.IsFiniteInFile = last == 'y';
                }

            }
            return automaton;
        }

        private Dictionary<string, bool> ReadWords(GraphVizFileModel graphVizFileModel, int lower, int upper)
        {
            var words = new Dictionary<string, bool>();
            for (int i = lower; i < upper; i++)
            {
                var line = graphVizFileModel.Lines[i];
                var chars = SplitOnComma(line);
                var isAccepted = chars[1] == "y";
                words.Add(chars[0], isAccepted);
            }
            return words;
        }

        private int FindUpperLimit(int currentIndex, List<string> lines)
        {
            for (var index = currentIndex; index < lines.Count; index++)
            {
                var line = lines[index];
                if (line.Contains("end."))
                {
                    return index;
                }
            }
            return -1;
        }

        private List<TransitionModel> ReadRegularAutomatonTransitions(GraphVizFileModel graphVizFileModel, int lower, int upper, AutomatonModel automaton)
        {
            var transitions = new List<TransitionModel>();
            for (int i = lower; i < upper; i++)
            {
                var transition = new TransitionModel();

                var transitionString = graphVizFileModel.Lines[i];
                var beginStateString = transitionString.Substring(0, transitionString.IndexOf(',')).Trim();
                var valueString =
                    transitionString.Substring(transitionString.IndexOf(',') + 1, transitionString.IndexOf('-') - 2)
                        .Trim();
                var endStateString = transitionString.Substring(transitionString.IndexOf('>') + 1).Trim();

                foreach (var state in automaton.States)
                {
                    if (state.Name.Equals(beginStateString) && i == lower)
                    {
                        transition.BeginState = state;
                        state.IsInitial = true;
                    }
                    else if (state.Name.Equals(beginStateString))
                    {
                        transition.BeginState = state;
                    }
                    if (state.Name.Equals(endStateString)) transition.EndState = state;
                    if (transition.BeginState != null && transition.EndState != null) break;
                }
                valueString = valueString.ToLower();
                if (valueString.Equals("_")) valueString = "ε";
                transition.Value = valueString;

                transitions.Add(transition);
            }
            return transitions;
        }

        private List<TransitionModel> ReadPdaTransitions(GraphVizFileModel graphVizFileModel, int lower, int upper, AutomatonModel automaton)
        {
            var transitions = new List<TransitionModel>();
            for (int i = lower; i < upper; i++)
            {
                var transition = new TransitionModel();

                var transitionString = graphVizFileModel.Lines[i];
                var beginStateString = transitionString.Substring(0, transitionString.IndexOf(',')).Trim();

                string valueString = "", leftStackString = "", rightStackString = ""; 
                if (transitionString.Contains("["))
                {
                    valueString =
                        transitionString.Substring(transitionString.IndexOf(',') + 1, transitionString.IndexOf('[') - 2)
                            .Trim();
                    leftStackString = transitionString.Substring(transitionString.IndexOf('[') + 1,
                        transitionString.IndexOf(',')).ToLower();

                    rightStackString = transitionString.Substring(transitionString.IndexOf(',', transitionString.IndexOf(',') + 1) + 1, 1).ToLower();
                }
                else
                {
                    valueString =
                        transitionString.Substring(transitionString.IndexOf(',') + 1, transitionString.IndexOf(',') + 3)
                            .Trim();
                    leftStackString = "_";
                    rightStackString = "_";
                }
                var endStateString = transitionString.Substring(transitionString.IndexOf('>') + 1).Trim();

                foreach (var state in automaton.States)
                {
                    if (state.Name.Equals(beginStateString) && i == lower)
                    {
                        transition.BeginState = state;
                        state.IsInitial = true;
                    }
                    else if (state.Name.Equals(beginStateString))
                    {
                        transition.BeginState = state;
                    }
                    if (state.Name.Equals(endStateString)) transition.EndState = state;
                    if (transition.BeginState != null && transition.EndState != null) break;
                }
                valueString = valueString.ToLower();
                if (valueString.Equals("_")) valueString = "ε";
                if (leftStackString.Equals("_")) leftStackString = "ε";
                if (rightStackString.Equals("_")) rightStackString = "ε";

                transition.PushStack = rightStackString;
                transition.PopStack = leftStackString;
                transition.Value = valueString;
                transitions.Add(transition);
            }
            return transitions;
        }


        public void WriteGraphVizFileToDotFile(List<string> lines)
        {
            using (StreamWriter sw = new StreamWriter("C:\\Program Files (x86)\\Graphviz2.38\\bin\\dot.dot", false))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }
        }

       public GraphVizFileModel ConvertAutomatonToGenericFile(AutomatonModel automaton)
        {
            var graphVizFile = new GraphVizFileModel();

            //ALPHABET
            string alphabetLine = "";
            foreach (var letter in automaton.Alphabet)
            {
                alphabetLine += letter;
            }
            graphVizFile.Lines.Add(alphabet + " " + alphabetLine);

            //STATES
            string statesLine = "";
            for (int index = 0; index < automaton.States.Count; index++)
            {
                var automatonState = automaton.States[index];
                if (index == automaton.States.Count - 1) statesLine += automatonState.Name;
                else statesLine += automatonState.Name + ",";
            }
            graphVizFile.Lines.Add(states + " " + statesLine);

            //FINAL
            var finalStates = automaton.States.Where(x => x.IsFinal);
            var finalLine = "";
            foreach (var finalState in finalStates)
            {
                finalLine += finalState.Name;
            }
            graphVizFile.Lines.Add(final + " " + finalLine);

            //TRANSITIONS
            graphVizFile.Lines.Add("transitions:");
            foreach (var automatonTransition in automaton.Transitions)
            {
                var transitionLine =
                    $"{automatonTransition.BeginState.Name},{automatonTransition.Value} --> {automatonTransition.EndState.Name}";
                graphVizFile.Lines.Add(transitionLine);
            }

            //END.
            graphVizFile.Lines.Add("end.");

            return graphVizFile;
        }

        private string GetSubstring(string line, string splitter)
        {
            var chars = line.Substring(line.IndexOf(splitter, StringComparison.InvariantCultureIgnoreCase) + splitter.Length);
            return chars.Trim();
        }

        private List<string> SplitOnComma(string chars)
        {
            return chars.Split(',').ToList();
        }
    }

}

