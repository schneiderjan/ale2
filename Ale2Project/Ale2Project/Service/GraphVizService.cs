using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ale2Project.Service
{
    public class GraphVizService : IGraphVizService
    {
        public void DisplayAutomaton()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var dotPath = path + @"\dot.dot";
            var imgPath = path + @"\dot.png";

            using (var p = new Process())
            {
                p.StartInfo.Verb = "runas";
                p.StartInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
                p.StartInfo.Arguments = " -Tpng -odot.png " + dotPath;
                p.StartInfo.ErrorDialog = true;
                p.Start();
                p.WaitForExit();
            }

            using (var p = new Process())
            {
                p.StartInfo.FileName = imgPath;
                p.StartInfo.ErrorDialog = true;
                p.Start();
            }

        }
    }
}
