using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLab4.Models
{
    public class TruthTableRowViewModel
    {
        private IReadOnlyList<bool> Inputs { get; }
        public bool Result { get; }

        // Конструктор
        public TruthTableRowViewModel(IReadOnlyList<bool> inputs, bool result)
        {
            Inputs = inputs;
            Result = result;
        }

        // Динамические свойства для каждой переменной
        public string? A => Inputs.Count > 0 ? (Inputs[0] ? "1" : "0") : null;
        public string? B => Inputs.Count > 1 ? (Inputs[1] ? "1" : "0") : null;
        public string? C => Inputs.Count > 2 ? (Inputs[2] ? "1" : "0") : null;
        public string? D => Inputs.Count > 3 ? (Inputs[3] ? "1" : "0") : null;
        public string? E => Inputs.Count > 4 ? (Inputs[4] ? "1" : "0") : null;

        public string Результат => Result ? "1" : "0";

        // Метод для получения количества используемых переменных
        public int VariablesCount => Inputs.Count;
    }
}
