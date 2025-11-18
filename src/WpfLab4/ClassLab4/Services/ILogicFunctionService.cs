using ClassLab4.Models;

namespace LogicFunctions.Core.Services
{
    public interface ILogicFunctionService
    {
        LogicalFunction CreateFromFormula(string formula);
        LogicalFunction CreateFromNumber(int variablesCount, int functionNumber);
        bool AreEquivalent(LogicalFunction f1, LogicalFunction f2);
    }

    public class LogicFunctionService : ILogicFunctionService
    {
        public LogicalFunction CreateFromFormula(string formula)
        {
            return LogicalFunction.FromFormula(formula);
        }

        public LogicalFunction CreateFromNumber(int variablesCount, int functionNumber)
        {
            return LogicalFunction.FromNumber(variablesCount, functionNumber);
        }

        public bool AreEquivalent(LogicalFunction f1, LogicalFunction f2)
        {
            return f1?.IsEquivalentTo(f2) ?? false;
        }
    }
}
