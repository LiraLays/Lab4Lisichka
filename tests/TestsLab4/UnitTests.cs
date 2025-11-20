using ClassLab4.Models;
using WpfLab4;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LogicFunctions.Tests
{
    public class LogicalFunctionTests
    {
        private readonly ILogicFunctionService _service;

        public LogicalFunctionTests()
        {
            _service = new LogicalFunctionService();
        }

        public class FormulaParserTests : LogicalFunctionTests
        {
            [Theory]
            [InlineData("A & B")]
            [InlineData("A | B")]
            [InlineData("!A")]
            [InlineData("(A & B) | C")]
            [InlineData("!(A & B)")]
            [InlineData("A & B & C")]
            public void Parse_ValidFormulas_ShouldNotThrow(string formula)
            {
                // Act & Assert
                var exception = Record.Exception(() => _service.CreateFromFormula(formula));
                Assert.Null(exception);
            }

            [Theory]
            //Правильные тесты
            [InlineData("A && B")]      // Двойной &
            [InlineData("A and B")]     // Текстовый оператор
            [InlineData("A B")]         // Отсутствует оператор

            // Заведомо неправильные тесты
            [InlineData("A &")]         // Неполное выражение
            [InlineData(") A & B (")]   // Неправильные скобки
            [InlineData("A & F")]       // Неправильная переменная
            public void Parse_InvalidFormulas_ShouldThrow(string formula)
            {
                // Act & Assert
                Assert.ThrowsAny<Exception>(() => _service.CreateFromFormula(formula));
            }

            [Theory]
            [InlineData("A & B", 2)]
            [InlineData("A | B | C", 3)]
            [InlineData("!(A & B)", 2)]
            [InlineData("A", 1)]
            public void CreateFromFormula_ValidInput_ShouldReturnCorrectVariablesCount(string formula, int expectedCount)
            {
                // Act
                var function = _service.CreateFromFormula(formula);

                // Assert
                Assert.Equal(expectedCount, function.VariablesCount);
            }
        }

        public class FunctionFromNumberTests : LogicalFunctionTests
        {
            [Theory]
            [InlineData(0, 0)]      // Неверное количество переменных
            [InlineData(6, 0)]      // Слишком много переменных
            [InlineData(2, -1)]     // Отрицательный номер
            [InlineData(2, 16)]     // Номер вне диапазона для 2 переменных
            public void CreateFromNumber_InvalidInput_ShouldThrow(int variablesCount, int functionNumber)
            {
                // Act & Assert
                Assert.ThrowsAny<Exception>(() => _service.CreateFromNumber(variablesCount, functionNumber));
            }
        }

        public class TruthTableTests : LogicalFunctionTests
        {
            [Fact]
            public void TruthTable_AndFunction_ShouldHaveCorrectResults()
            {
                // Arrange
                var function = _service.CreateFromFormula("A & B");

                // Act
                var truthTable = function.TruthTable;

                // Assert
                Assert.Equal(4, truthTable.Count);

                // A=0, B=0 -> 0
                Assert.False(truthTable[0].Result);
                // A=0, B=1 -> 0
                Assert.False(truthTable[1].Result);
                // A=1, B=0 -> 0
                Assert.False(truthTable[2].Result);
                // A=1, B=1 -> 1
                Assert.True(truthTable[3].Result);
            }

            [Fact]
            public void TruthTable_OrFunction_ShouldHaveCorrectResults()
            {
                // Arrange
                var function = _service.CreateFromFormula("A | B");

                // Act
                var truthTable = function.TruthTable;

                // Assert
                Assert.Equal(4, truthTable.Count);

                // Проверяем, что только первая строка дает false
                Assert.False(truthTable[0].Result); // 0,0
                Assert.True(truthTable[1].Result);  // 0,1
                Assert.True(truthTable[2].Result);  // 1,0
                Assert.True(truthTable[3].Result);  // 1,1
            }

            [Fact]
            public void TruthTable_ShouldHaveCorrectRowCount()
            {
                // Arrange
                var function1 = _service.CreateFromFormula("A");
                var function2 = _service.CreateFromFormula("A & B");
                var function3 = _service.CreateFromFormula("A & B & C");

                // Assert
                Assert.Equal(2, function1.TruthTable.Count);   // 2^1
                Assert.Equal(4, function2.TruthTable.Count);   // 2^2
                Assert.Equal(8, function3.TruthTable.Count);   // 2^3
            }
        }

        public class NormalFormsTests : LogicalFunctionTests
        {
            [Fact]
            public void GetDnf_AndFunction_ShouldReturnCorrectDnf()
            {
                // Arrange
                var function = _service.CreateFromFormula("A & B");

                // Act
                var dnf = function.GetDnf();

                // Assert
                Assert.Contains("A", dnf);
                Assert.Contains("B", dnf);
                Assert.Contains("∧", dnf); // И
            }

            [Fact]
            public void GetKnf_OrFunction_ShouldReturnCorrectKnf()
            {
                // Arrange
                var function = _service.CreateFromFormula("A | B");

                // Act
                var knf = function.GetKnf();

                // Assert
                Assert.Contains("A", knf);
                Assert.Contains("B", knf);
                Assert.Contains("∨", knf); // ИЛИ
            }

            [Fact]
            public void GetDnf_ConstantTrue_ShouldReturnConstant()
            {
                // Arrange
                var function = _service.CreateFromFormula("A | !A"); // Всегда истина

                // Act
                var dnf = function.GetDnf();

                // Assert
                Assert.Equal("1", dnf);
            }

            [Fact]
            public void GetKnf_ConstantFalse_ShouldReturnConstant()
            {
                // Arrange
                var function = _service.CreateFromFormula("A & !A"); // Всегда ложь

                // Act
                var knf = function.GetKnf();

                // Assert
                Assert.Equal("0", knf);
            }
        }

        public class EquivalenceTests : LogicalFunctionTests
        {
            [Fact]
            public void AreEquivalent_IdenticalFunctions_ShouldReturnTrue()
            {
                // Arrange
                var function1 = _service.CreateFromFormula("A & B");
                var function2 = _service.CreateFromFormula("A & B");

                // Act
                var result = _service.AreEquivalent(function1, function2);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void AreEquivalent_DifferentFunctions_ShouldReturnFalse()
            {
                // Arrange
                var function1 = _service.CreateFromFormula("A & B");
                var function2 = _service.CreateFromFormula("A | B");

                // Act
                var result = _service.AreEquivalent(function1, function2);

                // Assert
                Assert.False(result);
            }

            [Theory]
            [InlineData("A & B", "!( !A | !B )")]      // Закон де Моргана
            [InlineData("A | B", "!( !A & !B )")]      // Закон де Моргана
            public void AreEquivalent_EquivalentFunctions_ShouldReturnTrue(string formula1, string formula2)
            {
                // Arrange
                var function1 = _service.CreateFromFormula(formula1);
                var function2 = _service.CreateFromFormula(formula2);

                // Act
                var result = _service.AreEquivalent(function1, function2);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void AreEquivalent_NullFunction_ShouldReturnFalse()
            {
                // Arrange
                var function1 = _service.CreateFromFormula("A & B");

                // Act
                var result1 = _service.AreEquivalent(function1, null);
                var result2 = _service.AreEquivalent(null, function1);
                var result3 = _service.AreEquivalent(null, null);

                // Assert
                Assert.False(result1);
                Assert.False(result2);
                Assert.False(result3);
            }
        }

        public class CostTests : LogicalFunctionTests
        {
            [Fact]
            public void GetCost_SimpleFunction_ShouldReturnPositiveValue()
            {
                // Arrange
                var function = _service.CreateFromFormula("A & B");

                // Act
                var cost = function.GetCost();

                // Assert
                Assert.True(cost > 0);
            }

            [Fact]
            public void GetCost_ComplexFunction_ShouldHaveHigherCost()
            {
                // Arrange
                var simpleFunction = _service.CreateFromFormula("A & B");
                var complexFunction = _service.CreateFromFormula("(A & B) | (!A & !B) | C");

                // Act
                var simpleCost = simpleFunction.GetCost();
                var complexCost = complexFunction.GetCost();

                // Assert
                Assert.True(complexCost > simpleCost);
            }
        }

        public class IntegrationTests : LogicalFunctionTests
        {
            [Fact]
            public void FullWorkflow_ShouldWorkCorrectly()
            {
                // Arrange
                var formula = "(A & B) | C";

                // Act
                var function = _service.CreateFromFormula(formula);
                var truthTable = function.TruthTable;
                var dnf = function.GetDnf();
                var knf = function.GetKnf();
                var cost = function.GetCost();

                // Assert
                Assert.NotNull(function);
                Assert.Equal(3, function.VariablesCount);
                Assert.Equal(8, truthTable.Count); // 2^3 = 8 строк
                Assert.False(string.IsNullOrEmpty(dnf));
                Assert.False(string.IsNullOrEmpty(knf));
                Assert.True(cost > 0);
            }

            [Fact]
            public void CompareFormulaAndNumber_ShouldBeEquivalent()
            {
                // Arrange
                // Номер 6 для 2 переменных - это XOR
                var functionFromNumber = _service.CreateFromNumber(2, 6);
                var functionFromFormula = _service.CreateFromFormula("(A & !B) | (!A & B)");

                // Act
                var areEquivalent = _service.AreEquivalent(functionFromNumber, functionFromFormula);

                // Assert
                Assert.True(areEquivalent);
            }
        }
    }
}