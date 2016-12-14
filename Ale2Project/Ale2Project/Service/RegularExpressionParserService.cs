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
        private int _stateCount = 0;

        private const string Asterix = "*";
        private const string Or = "|";
        private const string Dot = ".";

        private AutomatonModel _automaton;

        public AutomatonModel GetAutomaton(string input)
        {
            _automaton = new AutomatonModel();

            var nodes = ParseRegularExpression(input);
            _automaton = NodesToAutomaton(nodes);
            _stateCount = 0;

            return _automaton;
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
            if (currentNode.Value == Dot)
            {
                if (previousNode == null) //initial aut.
                {
                    var state1 = new StateModel { IsInitial = true, IsFinal = false, Name = _stateCount++.ToString() };
                    var state2 = new StateModel { IsInitial = false, IsFinal = false, Name = _stateCount++.ToString() };
                    var state3 = new StateModel { IsInitial = false, IsFinal = true, Name = _stateCount++.ToString() };

                    var dotAutomaton = new AutomatonModel()
                    {
                        States = { state1, state2, state3 },
                    };
                    _automaton = dotAutomaton;

                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                        BuildAutomaton(currentNode.LeftChild, currentNode, state1, state2);
                    else
                        _automaton.Transitions.Add(new TransitionModel
                        { BeginState = state1, EndState = state2, Value = currentNode.LeftChild.Value });

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                        BuildAutomaton(currentNode.RightChild, currentNode, state2, state3);
                    else
                        _automaton.Transitions.Add(new TransitionModel
                        { BeginState = state2, EndState = state3, Value = currentNode.RightChild.Value });
                }
                else
                {
                    var middleState = new StateModel { Name = _stateCount++.ToString() };
                    _automaton.States.Add(middleState);

                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                    {
                        BuildAutomaton(currentNode.LeftChild, currentNode, leftState, middleState);
                    }
                    else
                    {
                        if (currentNode.LeftChild != null)
                        {
                            var trans = new TransitionModel
                            {
                                BeginState = leftState,
                                EndState = middleState,
                                Value = currentNode.LeftChild.Value
                            };
                            _automaton.Transitions.Add(trans);
                        }
                    }

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                    {
                        BuildAutomaton(currentNode.RightChild, currentNode, middleState, rightState);
                    }
                    else // in the value
                    {
                        if (currentNode.RightChild != null)
                        {
                            var trans = new TransitionModel()
                            {
                                BeginState = middleState,
                                EndState = rightState,
                                Value = currentNode.RightChild.Value
                            };
                            _automaton.Transitions.Add(trans);
                        }
                    }
                }

            }
            #endregion

            #region asterix
            else if (currentNode.Value == Asterix)
            {
                if (previousNode == null) //initial aut.
                {
                    var state1 = new StateModel { IsFinal = false, IsInitial = true, Name = _stateCount++.ToString() };
                    var state2 = new StateModel { IsFinal = false, IsInitial = false, Name = _stateCount++.ToString() };
                    var state3 = new StateModel { IsFinal = false, IsInitial = false, Name = _stateCount++.ToString() };
                    var state4 = new StateModel { IsFinal = true, IsInitial = false, Name = _stateCount++.ToString() };
                    var trans1 = new TransitionModel { BeginState = state1, EndState = state2, Value = "ε" };
                    var trans3 = new TransitionModel { BeginState = state3, EndState = state4, Value = "ε" };
                    var trans4 = new TransitionModel { BeginState = state1, EndState = state4, Value = "ε" };
                    var trans5 = new TransitionModel { BeginState = state3, EndState = state2, Value = "ε" };

                    var asterixAutomaton = new AutomatonModel()
                    {
                        States = { state1, state2, state3, state4 },
                        Transitions = { trans1, trans3, trans4, trans5 }
                    };
                    _automaton = asterixAutomaton;

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                        BuildAutomaton(currentNode.RightChild, currentNode, state2, state3);
                    else
                        _automaton.Transitions.Add(new TransitionModel()
                        { BeginState = state2, EndState = state3, Value = currentNode.RightChild.Value });
                }
                else
                {
                    var state1 = new StateModel { IsFinal = false, IsInitial = false, Name = _stateCount++.ToString() };
                    var state2 = new StateModel { IsFinal = false, IsInitial = false, Name = _stateCount++.ToString() };
                    var state3 = new StateModel { IsFinal = false, IsInitial = false, Name = _stateCount++.ToString() };
                    var state4 = new StateModel { IsFinal = false, IsInitial = false, Name = _stateCount++.ToString() };

                    var transLeftTo1 = new TransitionModel { BeginState = leftState, EndState = state1, Value = "ε" };
                    var trans4ToRight = new TransitionModel { BeginState = state4, EndState = rightState, Value = "ε" };
                    var trans1To2 = new TransitionModel { BeginState = state1, EndState = state2, Value = "ε" };
                    var trans1To4 = new TransitionModel { BeginState = state1, EndState = state4, Value = "ε" };
                    var trans3To2 = new TransitionModel { BeginState = state3, EndState = state2, Value = "ε" };
                    var trans3To4 = new TransitionModel { BeginState = state3, EndState = state4, Value = "ε" };

                    _automaton.States.Add(state1);
                    _automaton.States.Add(state2);
                    _automaton.States.Add(state3);
                    _automaton.States.Add(state4);

                    _automaton.Transitions.Add(transLeftTo1);
                    _automaton.Transitions.Add(trans4ToRight);
                    _automaton.Transitions.Add(trans1To2);
                    _automaton.Transitions.Add(trans1To4);
                    _automaton.Transitions.Add(trans3To2);
                    _automaton.Transitions.Add(trans3To4);

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                    {
                        BuildAutomaton(currentNode.RightChild, currentNode, state2, state3);
                    }
                    else
                    {
                        if (currentNode.RightChild != null)
                        {
                            _automaton.Transitions.Add(new TransitionModel()
                            {
                                BeginState = state2,
                                EndState = state3,
                                Value = currentNode.RightChild.Value
                            });
                        }
                    }
                }

            }
            #endregion

            #region or
            else if (currentNode.Value == Or)
            {
                if (previousNode == null)
                {
                    var state1 = new StateModel { IsInitial = true, IsFinal = false, Name = _stateCount++.ToString() };
                    var state2 = new StateModel { IsInitial = false, IsFinal = true, Name = _stateCount++.ToString() };
                    var orAutomaton = new AutomatonModel
                    {
                        States = { state1, state2 }
                    };

                    _automaton = orAutomaton;

                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                        BuildAutomaton(currentNode.LeftChild, currentNode, state1, state2);
                    else _automaton.Transitions.Add(new TransitionModel
                    { BeginState = state1, EndState = state2, Value = currentNode.LeftChild.Value });

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                        BuildAutomaton(currentNode.RightChild, currentNode, state1, state2);
                    else _automaton.Transitions.Add(new TransitionModel
                    { BeginState = state1, EndState = state2, Value = currentNode.RightChild.Value });
                }
                else
                {
                    if (IsOperandOrAsterix(currentNode.LeftChild.Value))
                    {
                        BuildAutomaton(currentNode.LeftChild, currentNode, leftState, rightState);
                    }
                    else
                    {
                        _automaton.Transitions.Add(new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = rightState,
                            Value = currentNode.LeftChild.Value
                        });
                    }

                    if (IsOperandOrAsterix(currentNode.RightChild.Value))
                    {
                        BuildAutomaton(currentNode.RightChild, currentNode, leftState, rightState);
                    }
                    else
                    {
                        _automaton.Transitions.Add(new TransitionModel()
                        {
                            BeginState = leftState,
                            EndState = rightState,
                            Value = currentNode.RightChild.Value
                        });
                    }
                }
            }
            #endregion

            return _automaton;
        }

        private bool IsOperand(char i)
        {
            return operands.Contains(i.ToString());
        }

        private bool IsAsterix(char i)
        {
            return Asterix.Equals(i.ToString());
        }

        private bool IsOperandOrAsterix(string c)
        {
            if (Asterix.Equals(c) || operands.Contains(c)) return true;
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
