using ClassLab4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLab4
{
    public interface ILogicFunctionService
    {
        LogicalFunction CreateFromFormula(string formula);
        LogicalFunction CreateFromNumber(int variableCount, int functionNumber);
        bool AreEquivalent(LogicalFunction f1, LogicalFunction f2);
    }

    public class LogicalFunctionService: ILogicFunctionService
    {
        public LogicalFunction CreateFromFormula(string formula) 
        {
            return LogicalFunction.FromFormula(formula);
        }

        public LogicalFunction CreateFromNumber(int variableCount, int functionNumber)
        {
            return LogicalFunction.FromNumber(variableCount, functionNumber);
        }

        public bool AreEquivalent(LogicalFunction f1, LogicalFunction f2)
        {
            return f1?.IsEquivalentTo(f2) ?? false;
        }
    }
}
