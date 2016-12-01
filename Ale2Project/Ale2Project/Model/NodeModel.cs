using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ale2Project.Model
{
    public class NodeModel
    {
        public NodeModel LeftChild { get; set; }
        public NodeModel RightChild { get; set; }
        public string Value { get; set; }

        public NodeModel(string _val, NodeModel _leftChild, NodeModel _rightChild)
        {
            Value = _val;
            LeftChild = _leftChild;
            RightChild = _rightChild;
        }

        public NodeModel(string _val)
        {
            Value = _val;
        }

        public NodeModel(string _val, NodeModel _rightChild)
        {
            Value = _val;
            RightChild = _rightChild;
        }
    }
}
