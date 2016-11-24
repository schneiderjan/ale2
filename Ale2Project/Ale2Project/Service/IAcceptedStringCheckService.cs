using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface IAcceptedStringCheckService
    {
        bool IsAcceptedString(string input, AutomatonModel automaton);
    }
}