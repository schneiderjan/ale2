using System;
using System.Collections.Generic;
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
                    var transitions = new List<TransitionModel>();
                    var lower = graphVizFileModel.Lines.IndexOf(line) + 1;
                    var upper = graphVizFileModel.Lines.Count - 1;

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
                    automaton.Transitions = transitions;
                    break;
                }

            }
            return automaton;
        }



        public void WriteGraphVizFileToDotFile(List<string> lines)
        {
            using (StreamWriter sw = new StreamWriter("C:\\Program Files (x86)\\Graphviz2.38\\bin\\dot.dot", false))
            {
                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
                sw.Dispose();
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

