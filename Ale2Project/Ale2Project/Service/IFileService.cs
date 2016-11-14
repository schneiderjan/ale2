using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface IFileService
    {
        GraphVizFileModel ReadFile();
        AutomatonModel ParseGraphVizFile(GraphVizFileModel graphVizFileModel);
    }
}