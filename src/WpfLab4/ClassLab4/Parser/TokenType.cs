using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLab4.Parser
{
    public enum TokenType
    {
        Variable,
        And,         // &, ∧, and
        Or,          // |, ∨, or  
        Not,         // !, ¬, not
        LeftParen,   // (
        RightParen,  // )
        End
    }
}
