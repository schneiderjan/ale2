using System;
using System.Collections.Generic;
using System.Linq;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class FileService : IFileService
    {
        private int _i;

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
                    var upper = graphVizFileModel.Lines.Count - lower;

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
                            if (state.Name.Equals(beginStateString)) transition.BeginState = state;
                            else if (state.Name.Equals(endStateString)) transition.EndState = state;

                            if (transition.BeginState != null && transition.EndState != null) break;

                        }
                        valueString = valueString.ToLower();
                        if (valueString.Equals("_")) valueString = "E";
                        transition.Value = valueString;

                        transitions.Add(transition);
                    }
                    automaton.Transitions = transitions;
                    break;
                }

            }
            return automaton;
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

        private TransitionModel ReadTransition(string line)
        {
            TransitionModel transition = new TransitionModel();

            return transition;
        }
    }

}

