using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLab4.Models.AstNodes
{
    public abstract class Node
    {
        public abstract bool Evaluate(Dictionary<char, bool> variableValues);
    }

    public class VariableNode : Node
    {
        public char VariableName { get; }

        public VariableNode(char variableName)
        {
            VariableName = variableName;
        }

        public override bool Evaluate(Dictionary<char, bool> variableValues)
        {
            if (!variableValues.ContainsKey(VariableName))
                throw new KeyNotFoundException($"Переменная {VariableName} не определена");

            return variableValues[VariableName];
        }

        public override string ToString() => VariableName.ToString();
    }

    public class NotNode : Node
    {
        public Node Operand { get; }

        public NotNode(Node operand)
        {
            Operand = operand;
        }

        public override bool Evaluate(Dictionary<char, bool> variableValues)
        {
            return !Operand.Evaluate(variableValues);
        }

        public override string ToString() => $"!({Operand})";
    }

    public class AndNode : Node
    {
        public Node Left { get; }
        public Node Right { get; }

        public AndNode(Node left, Node right)
        {
            Left = left;
            Right = right;
        }

        public override bool Evaluate(Dictionary<char, bool> variableValues)
        {
            return Left.Evaluate(variableValues) && Right.Evaluate(variableValues);
        }

        public override string ToString() => $"({Left} & {Right})";
    }

    public class OrNode : Node
    {
        public Node Left { get; }
        public Node Right { get; }

        public OrNode(Node left, Node right)
        {
            Left = left;
            Right = right;
        }

        public override bool Evaluate(Dictionary<char, bool> variableValues)
        {
            return Left.Evaluate(variableValues) || Right.Evaluate(variableValues);
        }

        public override string ToString() => $"({Left} | {Right})";
    }
}
