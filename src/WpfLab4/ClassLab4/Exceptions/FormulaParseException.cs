using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLab4.Exceptions
{
    public class FormulaParseException : Exception
    {
        public FormulaParseException(string message) : base(message) { }
        public FormulaParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
