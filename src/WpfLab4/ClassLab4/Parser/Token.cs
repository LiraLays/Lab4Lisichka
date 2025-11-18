using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLab4.Parser
{
    public class Token
    {
        public TokenType Type { get; }
        public char Character { get; }
        public string Value {  get; }

        public Token(TokenType type, char character = '\0', string value = "")
        {
            Type = type;
            Character = character;
            Value = value;
        }

        public override string ToString() => $"{Type}({Character})";
    }
}
