using System.Collections.Generic;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public interface ILanguageCheckService
    {
        bool IsAcceptedString(string input, AutomatonModel automaton);
        List<string> GetAllWords(AutomatonModel automaton);
        bool IsAcceptedStringByPda(string input, AutomatonModel automaton);
    }
}