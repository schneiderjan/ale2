using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface IRegularExpressionParserService
    {
        AutomatonModel GetAutomaton(string input);
    }
}