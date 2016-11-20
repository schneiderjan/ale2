using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface IDfaCheckService
    {
        bool IsAutomatonNda(AutomatonModel automaton);
    }
}