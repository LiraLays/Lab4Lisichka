using System;
using System.Collections.Generic;
using System.Linq;
using ClassLab4.Models.AstNodes;
using ClassLab4.Parser;

namespace ClassLab4.Models
{
    public class LogicalFunction
    {
        private readonly Node _astRoot;
        private readonly HashSet<char> _variables;

        // Основные свойства
        public string Name { get; set; }
        public int VariablesCount => _variables.Count;
        public string Formula { get; private set; }

        // Таблица истинности - ключевой источник данных
        public IReadOnlyList<TruthTableRow> TruthTable { get; private set; }

        private LogicalFunction(Node astRoot, HashSet<char> variables, string formula)
        {
            _astRoot = astRoot;
            _variables = variables;
            Formula = formula;
            TruthTable = BuildTruthTable();
        }

        public static LogicalFunction FromFormula(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                throw new ArgumentException("Формула не может быть пустой");
            }

            var parser = new FormulaParcer(formula);
            var astRoot = parser.Parse();
            var variables = ExtractVariables(astRoot);

            return new LogicalFunction(astRoot, variables, formula);
        }

        public static LogicalFunction FromNumber(int variablesCount, int functionNumber)
        {
            if (variablesCount < 1 || variablesCount > 100)
            {
                throw new ArgumentException("Количество переменных должно быть от 1 до 10");
            }

            var maxFunctions = 1 << (1 << variablesCount);
            if (functionNumber < 0 || functionNumber >= maxFunctions)
            {
                throw new ArgumentException($"Номер функции должен быть от 0 до {maxFunctions - 1}");
            }

            // Создание таблицы истинности по номеру функции
            var variableNames = Enumerable.Range(0, variablesCount)
                .Select(i => (char)('A' + i))
                .ToArray();
            var truthTable = BuildTruthTableFromNumber(variableNames, functionNumber);
            var formula = GenerateFormulaFromTruthTable(truthTable, variableNames);

            return FromFormula(formula);
        }

        public static LogicalFunction FromTruthTable(int variablesCount, bool[] results)
        {
            // Реализация создания функции из готовой таблицы истинности
            // (аналогично FromNumber, но с готовыми результатами)
            throw new NotImplementedException();
        }

        public string GetDnf()
        {
            var trueRows = TruthTable.Where(row =>  row.Result).ToList();

            if (trueRows.Count == 0)
                return "0"; // Костанта false
            
            if (trueRows.Count == TruthTable.Count)
                return "1"; // Костанта true

            var terms = new List<string>();

            foreach (var row in trueRows)
            {
                var literals = new List<string>();
                for (int i = 0; i < VariablesCount; i++)
                {
                    var variableName = _variables.ElementAt(i);
                    if (row.Inputs[i])
                        literals.Add(variableName.ToString());
                    else
                        literals.Add($"¬{variableName}");
                }
                terms.Add($"({string.Join(" ∧ ", literals)})");
            }

            return string.Join(" ∨ ", terms);
        }

        public string GetKnf()
        {
            var falseRows = TruthTable.Where(row => !row.Result).ToList();

            if (falseRows.Count == 0)
                return "1"; // Константа true

            if (falseRows.Count == TruthTable.Count)
                return "0"; // Константа false

            var clauses = new List<string>();

            foreach (var row in falseRows)
            {
                var literals = new List<string>();
                for (int i = 0; i < VariablesCount; i++)
                {
                    var variableName = _variables.ElementAt(i);
                    if (!row.Inputs[i])
                        literals.Add(variableName.ToString());
                    else
                        literals.Add($"¬{variableName}");
                }
                clauses.Add($"({string.Join(" ∨ ", literals)})");
            }

            return string.Join(" ∧ ", clauses);
        }

        public bool IsEquivalentTo(LogicalFunction other)
        {
            if (other == null) return false;
            if (VariablesCount != other.VariablesCount) return false;

            // Сравниваем таблицы истинности
            for (int i = 0; i < TruthTable.Count; i++)
            {
                if (TruthTable[i].Result != other.TruthTable[i].Result)
                    return false;
            }

            return true;
        }

        public int GetCost()
        {
            var dnf = GetDnf();
            // Подсчитываем литералы (переменные с отрицанием или без)
            var literalCount = dnf.Count(c => char.IsLetter(c));
            literalCount += dnf.Count(c => c == '¬');

            // Подсчитываем конъюнкты и дизъюнкты
            var andCount = dnf.Count(c => c == '∧');
            var orCount = dnf.Count(c => c == '∨');

            return literalCount + andCount + orCount;
        }

        public bool Evaluate(Dictionary<char, bool> variableValues)
        {
            return _astRoot.Evaluate(variableValues);
        }

        private IReadOnlyList<TruthTableRow> BuildTruthTable()
        {
            var variableNames = _variables.OrderBy(v => v).ToArray();
            var table = new List<TruthTableRow>();
            var rowCount = 1 << VariablesCount; // 2^n

            for (int i = 0; i < rowCount; i++)
            {
                var inputs = new bool[VariablesCount];
                var variableValues = new Dictionary<char, bool>();

                // Создаем битовую маску для текущей строки
                for (int j = 0; j < VariablesCount; j++)
                {
                    var value = (i & (1 << (VariablesCount - 1 - j))) != 0;
                    inputs[j] = value;
                    variableValues[variableNames[j]] = value;
                }

                var result = _astRoot.Evaluate(variableValues);
                table.Add(new TruthTableRow(inputs, result));
            }

            return table;
        }

        private static HashSet<char> ExtractVariables(Node node)
        {
            var variables = new HashSet<char>();
            CollectVariables(node, variables);
            return variables;
        }

        private static void CollectVariables(Node node, HashSet<char> variables)
        {
            switch (node)
            {
                case VariableNode varNode:
                    variables.Add(varNode.VariableName);
                    break;
                case NotNode notNode:
                    CollectVariables(notNode.Operand, variables);
                    break;
                case AndNode andNode:
                    CollectVariables(andNode.Left, variables);
                    CollectVariables(andNode.Right, variables);
                    break;
                case OrNode orNode:
                    CollectVariables(orNode.Left, variables);
                    CollectVariables(orNode.Right, variables);
                    break;
            }
        }

        private static List<TruthTableRow> BuildTruthTableFromNumber(char[] variableNames, int functionNumber)
        {
            var table = new List<TruthTableRow>();
            var VariablesCount = variableNames.Length;
            var rowCount = 1 << VariablesCount;

            for (int i = 0; i < rowCount; i++)
            {
                var inputs = new bool[VariablesCount];
                for (int j = 0; j < VariablesCount; j++)
                {
                    inputs[j] = (i & (1 << (VariablesCount - 1 - j))) != 0;
                }

                // Бит результата определяется номером функции
                var resultBit = (functionNumber >> (rowCount - 1 - i)) & 1;
                var result = resultBit == 1;

                table.Add(new TruthTableRow(inputs, result));
            }

            return table;
        }

        private static string GenerateFormulaFromTruthTable(List<TruthTableRow> truthTable, char[] variableNames)
        {
            // Генерируем DNF из таблицы истинности для создания формулы
            var trueRows = truthTable.Where(row => row.Result).ToList();

            if (trueRows.Count == 0) return "0";
            if (trueRows.Count == truthTable.Count) return "1";

            var terms = new List<string>();

            foreach (var row in trueRows)
            {
                var literals = new List<string>();
                for (int i = 0; i < variableNames.Length; i++)
                {
                    if (row.Inputs[i])
                        literals.Add(variableNames[i].ToString());
                    else
                        literals.Add($"!{variableNames[i]}");
                }
                terms.Add($"({string.Join(" & ", literals)})");
            }

            return string.Join(" | ", terms);
        }

        public override string ToString() => Formula ?? "Безымянная функция";
    }
}
