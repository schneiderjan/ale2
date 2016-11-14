using System.Collections.Generic;
using System.Windows.Documents;
using GalaSoft.MvvmLight;

namespace Ale2Project.Model
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GraphVizFile : ViewModelBase
    {
        public string FilePath { get; set; }
        public List<string> Lines { get; set; }
    }
}