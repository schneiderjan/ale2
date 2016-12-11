using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class GraphVizService : IGraphVizService
    {
        public void DisplayAutomaton()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var dotPath = path + @"\dot.dot";
            var imgPath = path + @"\dot.png";

            //using (var p = new Process())
            //{
            //    p.StartInfo.Verb = "runas";
            //    p.StartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
            //    p.StartInfo.Arguments = " -Tpng -odot.png " + dotPath;
            //    p.StartInfo.ErrorDialog = true;
            //    p.Start();
            //    p.WaitForExit();
            //}

            //using (var p = new Process())
            //{
            //    p.StartInfo.FileName = imgPath;
            //    p.StartInfo.ErrorDialog = true;
            //    p.Start();
            //}
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = @"C:\Program Files (x86)\Graphviz2.38\bin";
            processStartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
            processStartInfo.Arguments = "-Tpng -odot.png dot.dot";
            processStartInfo.ErrorDialog = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;

            Process p = Process.Start(processStartInfo);
        }

        public GraphVizFileModel ConvertToGraphVizFile(AutomatonModel automaton)
        {
            var file = new GraphVizFileModel();
            file.Lines.Add("digraph myAutomaton {");
            file.Lines.Add("rankdir=LR;");
            file.Lines.Add("\"\"[shape = none]");

            //Add initials
            foreach (var automatonState in automaton.States)
            {
                if (automatonState.IsFinal && automatonState.IsInitial)
                {
                    file.Lines.Add($"\"{automatonState.Name}\" [shape=doublecircle]");
                    file.Lines.Add($"\"\" -> \"{automatonState.Name}\"");
                }
                else if (automatonState.IsFinal)
                {
                    file.Lines.Add($"\"{automatonState.Name}\" [shape=doublecircle]");
                }
                else if (automatonState.IsInitial)
                {
                    file.Lines.Add($"\"{automatonState.Name}\" [shape=circle]");
                    file.Lines.Add($"\"\" -> \"{automatonState.Name}\"");
                }
                else
                {
                    file.Lines.Add($"\"{automatonState.Name}\" [shape=circle]");
                }
            }

            //add transitions
            foreach (var transition in automaton.Transitions)
            {
                file.Lines.Add($"\"{transition.BeginState.Name}\" -> \"{transition.EndState.Name}\" [label=\"{transition.Value}\"]");
            }

            file.Lines.Add("}");

            return file;
        }
    }
}
