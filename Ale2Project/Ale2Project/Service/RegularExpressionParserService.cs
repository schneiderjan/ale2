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
        private int stateCount = 0;

        private string asterix = "*";
        private string or = "|";
        private string dot = ".";

        private AutomatonModel automaton;

        public AutomatonModel GetAutomaton(string input)
        {
            automaton = new AutomatonModel();

            var nodes = ParseRegularExpression(input);
            automaton = NodesToAutomaton(nodes);

            return automaton;
        }

        private AutomatonModel NodesToAutomaton(List<NodeModel> nodes)
        {
            nodes.Reverse();
            //make recursive call buildAutomaton(thisNode, previousNode)
            return BuildAutomaton(nodes[0], null, null, null);
        }

        private AutomatonModel BuildAutomaton(NodeModel currentNode, NodeModel previousNode, StateModel leftState, StateModel rightState)
        {
            if (currentNode == null) return null;

            #region dot
            if (currentNode.Value == dot)
            {
                if (previousNode == null) //initial aut.
                {
                    var state1 = new StateModel { IsInitial = true, IsFinal = false, Name = stateCount++.ToString() };
                    var state2 = new StateModel { IsInitial = false, IsFinal = false, Name = stateCount++.ToString() };
                    var state3 = new StateModel { IsInitial = false, IsFinal = true, Name = stateCount++.ToString() };
                    //var trans1 = new TransitionModel { BeginState = state1, EndState = state2, Value = "X" };
                    //var trans2 = new TransitionModel { BeginState = state2, EndState = state3, Value = "Y" };
                    var dotAutomaton = new AutomatonModel()
                    {
                        States = { state1, state2, state3 },
                        //Transitions = { trans1, trans2 }
                    };
                    automaton = dotAutomaton;
                    leftState = state1;
                    rightState = state3;
                }
                else
                {
                    var middleState = new StateModel { Name = stateCount++.ToString() };


                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                    {
                        rightState = middleState;
                        BuildAutomaton(currentNode.LeftChild, currentNode, leftState, rightState);
                    }
                    else
                    {
                        var trans = new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = middleState,
                            Value = currentNode.LeftChild.Value
                        };
                        automaton.Transitions.Add(trans);
                    }

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                    {
                        leftState = middleState;
                        BuildAutomaton(currentNode.RightChild, currentNode, leftState, rightState);
                    }
                    else // in the value
                    {
                        var trans = new TransitionModel()
                        {
                            BeginState = middleState,
                            EndState = rightState,
                            Value = currentNode.RightChild.Value
                        };
                        automaton.Transitions.Add(trans);
                    }
                }

            }
            #endregion

            #region asterix
            else if (currentNode.Value == asterix)
            {
                if (previousNode == null) //initial aut.
                {
                    var state1 = new StateModel { IsFinal = false, IsInitial = true, Name = stateCount++.ToString() };
                    var state2 = new StateModel { IsFinal = false, IsInitial = false, Name = stateCount++.ToString() };
                    var state3 = new StateModel { IsFinal = false, IsInitial = false, Name = stateCount++.ToString() };
                    var state4 = new StateModel { IsFinal = true, IsInitial = false, Name = stateCount++.ToString() };
                    var trans1 = new TransitionModel { BeginState = state1, EndState = state2, Value = "ε" };
                    //var trans2 = new TransitionModel { BeginState = state2, EndState = state3, Value = "X" };
                    var trans3 = new TransitionModel { BeginState = state3, EndState = state4, Value = "ε" };
                    var trans4 = new TransitionModel { BeginState = state1, EndState = state4, Value = "ε" };
                    var trans5 = new TransitionModel { BeginState = state3, EndState = state2, Value = "ε" };

                    var asterixAutomaton = new AutomatonModel()
                    {
                        States = { state1, state2, state3, state4 },
                        Transitions = { trans1, trans3, trans4, trans5 }
                    };
                    automaton = asterixAutomaton;
                    leftState = state2;
                    rightState = state3;
                }
                else
                {
                    //eventually its the rightChile
                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                    {
                        var state1 = new StateModel { IsFinal = false, IsInitial = false, Name = stateCount++.ToString() };
                        var state2 = new StateModel { IsFinal = false, IsInitial = false, Name = stateCount++.ToString() };
                        var state3 = new StateModel { IsFinal = false, IsInitial = false, Name = stateCount++.ToString() };
                        var state4 = new StateModel { IsFinal = false, IsInitial = false, Name = stateCount++.ToString() };
                        var trans1 = new TransitionModel { BeginState = state1, EndState = state2, Value = "ε" };
                        //var trans2 = new TransitionModel { BeginState = state2, EndState = state3, Value = "X" };
                        var trans3 = new TransitionModel { BeginState = state3, EndState = state4, Value = "ε" };
                        var trans4 = new TransitionModel { BeginState = state1, EndState = state4, Value = "ε" };
                        var trans5 = new TransitionModel { BeginState = state3, EndState = state2, Value = "ε" };

                        //transition from left to state1
                        //transition from right to state4
                        var transLeftToState1 = new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = state1,
                            Value = "ε"
                        };
                        var transState4ToRight = new TransitionModel()
                        {
                            BeginState = state4,
                            EndState = rightState,
                            Value = "ε"
                        };

                        automaton.States.Add(state1);
                        automaton.States.Add(state2);
                        automaton.States.Add(state3);
                        automaton.States.Add(state4);
                        automaton.Transitions.Add(trans1);
                        automaton.Transitions.Add(trans3);
                        automaton.Transitions.Add(trans4);
                        automaton.Transitions.Add(trans5);
                        automaton.Transitions.Add(transLeftToState1);
                        automaton.Transitions.Add(transState4ToRight);

                        leftState = state2;
                        rightState = state3;
                        BuildAutomaton(currentNode.RightChild, currentNode, leftState, rightState);
                    }
                    else
                    {
                        automaton.Transitions.Add(new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = rightState,
                            Value = currentNode.LeftChild.Value
                        });
                    }
                }

            }
            #endregion

            #region or
            else if (currentNode.Value == or)
            {
                if (previousNode == null)
                {
                    var state1 = new StateModel { IsInitial = true, IsFinal = false, Name = stateCount++.ToString() };
                    var state2 = new StateModel { IsInitial = false, IsFinal = true, Name = stateCount++.ToString() };
                    //var trans1 = new TransitionModel { BeginState = state1, EndState = state2, Value = "X" };
                    //var trans2 = new TransitionModel { BeginState = state1, EndState = state2, Value = "Y" };

                    var orAutomaton = new AutomatonModel
                    {
                        //Transitions = { trans1, trans2 },
                        States = { state1, state2 }
                    };
                    automaton = orAutomaton;
                    leftState = state1;
                    rightState = state2;
                }
                else
                {
                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                    {
                        var newleftState = new StateModel { Name = stateCount++.ToString() };
                        var transition = new TransitionModel
                        {
                            BeginState = leftState,
                            EndState = newleftState,
                            Value = "ε"
                        };
                        automaton.States.Add(newleftState);
                        automaton.Transitions.Add(transition);

                        BuildAutomaton(currentNode.LeftChild, currentNode, leftState, rightState);
                    }
                    else
                    {
                        automaton.Transitions.Add(new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = rightState,
                            Value = currentNode.LeftChild.Value
                        });
                    }

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                    {
                        var newRightState = new StateModel() { Name = stateCount++.ToString() };
                        var transition = new TransitionModel()
                        {
                            BeginState = rightState,
                            EndState = newRightState,
                            Value = "ε"
                        };
                        automaton.Transitions.Add(transition);
                        automaton.States.Add(newRightState);
                        rightState = newRightState;

                        BuildAutomaton(currentNode.RightChild, currentNode, leftState, rightState);
                    }
                    else
                    {
                        automaton.Transitions.Add(new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = rightState,
                            Value = currentNode.RightChild.Value
                        });
                    }
                }
            }
            #endregion


            stateCount = 0;
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

        private bool IsOperandOrAsterix(string c)
        {
            if (asterix.Equals(c) || operands.Contains(c)) return true;
            return false;
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
