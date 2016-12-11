using System.Collections.Generic;
using System.Windows.Documents;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface IFileService
    {
        GraphVizFileModel ReadFile();
        AutomatonModel ParseGraphVizFile(GraphVizFileModel graphVizFileModel);
        void WriteGraphVizFileToDotFile(List<string> lines);
        GraphVizFileModel ConvertAutomatonToGenericFile(AutomatonModel automaton);
    }
}