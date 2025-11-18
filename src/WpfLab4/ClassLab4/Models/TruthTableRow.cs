using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLab4.Models
{
    public class TruthTableRow
    {
        // Значения переменных для этой строки
        public IReadOnlyList<bool> Inputs { get; }
        // Результат функции для этого набора
        public bool Result { get; }

        public TruthTableRow(IReadOnlyList<bool> inputs, bool result)
        {
            Inputs = inputs;
            Result = result;
        }

        public override string ToString()
        {
            var inputsStr = string.Join(" ", Inputs.Select(x => x ? "1" : "0"));
            return $"{inputsStr} | {(Result ? "1" : "0")}";
        }
    }
}
