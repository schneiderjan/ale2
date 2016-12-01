using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ale2Project.Model;

namespace Ale2Project.Service
{
    public class RegularExpressionParserService : IRegularExpressionParserService
    {
        private List<string> operands = new List<string>()
        {
            "|",
            "."
        };

        private string asterix = "*";
        private string or = "|";
        private string dot = ".";

        public AutomatonModel GetAutomaton(string input)
        {
            var nodes = ParseRegularExpression(input);
            var automaton = NodesToAutomaton(nodes);


            return automaton;
        }

        private AutomatonModel NodesToAutomaton(List<NodeModel> nodes)
        {
            var automaton = new AutomatonModel();

            //make recursive call buildAutomaton(thisNode, previousNode)
            foreach (var node in nodes)
            {
                if (node.Value == dot)
                {
                    //check previous node
                    //create three states
                    //add to automatoin
                }
                else if (node.Value == asterix)
                {
                    //check previous node
                    //create four states and build transitions
                    //add to automaton
                }
                else if (node.Value == or)
                {
                    //check previous node
                    //create two states
                    //add to automaton
                }
            }
            return automaton;
        }

        private bool IsOperand(char i)
        {
            return operands.Contains(i.ToString());
        }

        private bool IsAsterix(char i)
        {
            return asterix.Equals(i.ToString());
        }

        private List<NodeModel> ParseRegularExpression(string input)
        {
            List<char> regularExpressionList = new List<char>();
            var stack = new Stack<NodeModel>();
            List<NodeModel> nodes = new List<NodeModel>();

            //Remove unnecessary chars
            input = input
                .Replace(",", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(@" ", "")
                .Trim();

            foreach (char i in input) regularExpressionList.Add(i);
            regularExpressionList.Reverse();

            foreach (var re in regularExpressionList)
            {
                if (IsOperand(re))
                {
                    NodeModel leftOperand = stack.Pop();
                    NodeModel rightOperand = stack.Pop();
                    var node = new NodeModel(re.ToString(), leftOperand, rightOperand);
                    stack.Push(node);
                    nodes.Add(node);
                }
                else if (IsAsterix(re))
                {
                    NodeModel rightOperand = stack.Pop();
                    var node = new NodeModel(re.ToString(), rightOperand);
                    nodes.Add(node);
                    stack.Push(node);
                }
                else
                {
                    var node = new NodeModel(re.ToString());
                    nodes.Add(node);
                    stack.Push(node);
                }
            }
            return nodes;
        }
    }
}
