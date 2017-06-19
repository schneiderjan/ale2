using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface IDfaService
    {
        bool IsAutomatonDfa(AutomatonModel automaton);
        AutomatonModel ConvertNdfaToDfa(AutomatonModel ndfa);
    }
}