using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Documents;
using ClassLab4.Models;
using System.Windows;

namespace WpfLab4
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static Dictionary<string, string> ERROR_MESSAGES = new Dictionary<string, string>
        {
            { "NoFunc", "Функция не задана" },
            { "Def2Funcs", "Задайте обе функции для проверки" }
        };
        private readonly ILogicFunctionService _logicService;

        public MainViewModel(ILogicFunctionService logicService)
        {
            _logicService = logicService;

            // Инициализация команд
            BuildFunction1Command = new RelayCommand(BuildFunction1, () => CanBuildFunction1);
            BuildFunction2Command = new RelayCommand(BuildFunction2, () => CanBuildFunction2);
            CheckEquivalenceCommand = new RelayCommand(CheckEquivalence, () => CanCheckEquivalence);

            // Инициализация новых команд
            BuildFunction1FromNumberCommand = new RelayCommand(() => BuildFunctionFromNumber(1), () => CanBuildFunction1);
            BuildFunction1FromFormulaCommand = new RelayCommand(() => BuildFunctionFromFormula(1), () => CanBuildFunction1);
            BuildFunction2FromNumberCommand = new RelayCommand(() => BuildFunctionFromNumber(2), () => CanBuildFunction2);
            BuildFunction2FromFormulaCommand = new RelayCommand(() => BuildFunctionFromFormula(2), () => CanBuildFunction2);

            // Инициализация свойств
            StatusMessage = "Готов к работе";
        }

        #region Вспомогательные методы для команд
            private void BuildFunctionFromNumber(int functionNumber)
            {
                try
                {
                    var input = functionNumber == 1 ? Function1Input : Function2Input;

                    if (int.TryParse(input, out int number))
                    {
                        int variablesCount = DetermineVariablesCount(number);
                        var function = _logicService.CreateFromNumber(variablesCount, number);

                        if (functionNumber == 1)
                        {
                            Function1 = function;
                            isFunction1Expanded = true;
                            StatusMessage = $"Функция 1 построена из номера {number}";
                        }
                        else
                        {
                            Function2 = function;
                            isFunction2Expanded = true;
                            StatusMessage = $"Функция 2 построена из номера {number}";
                        }
                    }
                    else
                    {
                        StatusMessage = "Ошибка: ввод не является числом";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка построения функции из номера: {ex.Message}";
                }
            }

            private void BuildFunctionFromFormula(int functionNumber)
            {
                try
                {
                    var input = functionNumber == 1 ? Function1Input : Function2Input;
                    var function = _logicService.CreateFromFormula(input);

                    if (functionNumber == 1)
                    {
                        Function1 = function;
                        isFunction1Expanded = true;
                        StatusMessage = $"Функция 1 построена из формулы";
                    }
                    else
                    {
                        Function2 = function;
                        isFunction2Expanded = true;
                        StatusMessage = $"Функция 2 построена из формулы";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка построения функции из формулы: {ex.Message}";
                }
            }
        #endregion

        #region Свойства для привязки данных

        // Входные данные
        private string _function1Input = "A & B";
        public string Function1Input
        {
            get => _function1Input;
            set
            {
                _function1Input = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _function2Input = "A | B";
        public string Function2Input
        {
            get => _function2Input;
            set
            {
                _function2Input = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        //Функции
        private LogicalFunction _function1;
        public LogicalFunction Function1
        {
            get => _function1;
            private set
            {
                _function1 = value;
                OnPropertyChanged();
                // При изменении функции обновление всех зависимых свойств
                OnPropertyChanged(nameof(TruthTable1));
                OnPropertyChanged(nameof(Dnf1));
                OnPropertyChanged(nameof(Knf1));
                OnPropertyChanged(nameof(Cost1));
                OnPropertyChanged(nameof(IsFunction1Valid));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private LogicalFunction _function2;
        public LogicalFunction Function2
        {
            get => _function2;
            private set
            {
                _function2 = value;
                OnPropertyChanged();
                // При изменении функции обновление всех зависимых свойств
                OnPropertyChanged(nameof(TruthTable2));
                OnPropertyChanged(nameof(Dnf2));
                OnPropertyChanged(nameof(Knf2));
                OnPropertyChanged(nameof(Cost2));
                OnPropertyChanged(nameof(IsFunction2Valid));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Результаты для Функции 1
        public IReadOnlyList<TruthTableRow> TruthTable1 => Function1?.TruthTable;
        public string Dnf1 => Function1?.GetDnf() ?? ERROR_MESSAGES["NoFunc"];
        public string Knf1 => Function1?.GetKnf() ?? ERROR_MESSAGES["NoFunc"];
        public int Cost1 => Function1?.GetCost() ?? 0;
        public bool IsFunction1Valid => Function1 != null;

        // Результаты для Функции 2
        public IReadOnlyList<TruthTableRow> TruthTable2 => Function2?.TruthTable;
        public string Dnf2 => Function2?.GetDnf() ?? ERROR_MESSAGES["NoFunc"];
        public string Knf2 => Function2?.GetKnf() ?? ERROR_MESSAGES["NoFunc"];
        public int Cost2 => Function2?.GetCost() ?? 0;
        public bool IsFunction2Valid => Function2 != null;

        // Эквивалентность
        // Обновление цвета при изменении эквивалентности
        private bool _isEquivalent;
        public bool IsEquivalent
        {
            get => _isEquivalent;
            private set
            {
                _isEquivalent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EquivalenceMessage));
                // Remove the line below since we're no longer using EquivalenceColor
                // OnPropertyChanged(nameof(EquivalenceColor));
            }
        }

        public string EquivalenceMessage
        {
            get
            {
                if (Function1 == null || Function2 == null)
                    return ERROR_MESSAGES["Def2Funcs"];

                return IsEquivalent ?
                    "Функции ЭКВИВАЛЕНТНЫ" :
                    "Функции не эквивалентны";
            }
        }

        // Свойство для цвета сообщения об эквивалентности
        //public Brush EquivalenceColor => IsEquivalent ?
        //    new SolidColorBrush(Colors.Green) :
         //   new SolidColorBrush(Colors.Red);

        // Состояние Expander'ов
        private bool _isFunсtion1Expanded = true;
        public bool isFunction1Expanded
        {
            get => _isFunсtion1Expanded;
            set
            {
                _isFunсtion1Expanded = value;
                OnPropertyChanged();
            }
        }

        private bool _isFunction2Expanded = false;
        public bool isFunction2Expanded
        {
            get => _isFunction2Expanded;
            set
            {
                _isFunction2Expanded = value;
                OnPropertyChanged();
            }
        }

        // Статус
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Команды

        public ICommand BuildFunction1Command { get; }
        public ICommand BuildFunction2Command { get; }
        public ICommand CheckEquivalenceCommand { get; }

        public ICommand BuildFunction1FromNumberCommand { get; }
        public ICommand BuildFunction1FromFormulaCommand { get; }
        public ICommand BuildFunction2FromNumberCommand { get; }
        public ICommand BuildFunction2FromFormulaCommand { get; }

        private bool CanBuildFunction1 => !string.IsNullOrWhiteSpace(Function1Input);
        private bool CanBuildFunction2 => !string.IsNullOrWhiteSpace(Function2Input);
        private bool CanCheckEquivalence => Function1 != null && Function2 != null;

        #endregion

        #region Реализация команд

        private void BuildFunction1()
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine("BuildFunction1 команда выполняется");
                //StatusMessage = "Строим функцию 1...";

                // Временно упростим логику для тестирования
                if (string.IsNullOrWhiteSpace(Function1Input))
                {
                    StatusMessage = "Ошибка: пустой ввод для функции 1";
                    return;
                }

                // Простая заглушка для тестирования
                //StatusMessage = $"Функция 1 обработана: {Function1Input}";
                //System.Diagnostics.Debug.WriteLine($"Обработан ввод: {Function1Input}");

                StatusMessage = "Построение функции 1...";

                // Попытка определить тип ввода и создать функцию
                Function1 = CreateFunctionFromInput(Function1Input);

                StatusMessage = $"Функция 1 построена. Переменных: {Function1.VariablesCount}";
                isFunction1Expanded = true; // Автоматически разворачиваются результаты
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"Ошибка в BuildFunction1: {ex.Message}");
                StatusMessage = $"Ошибка построения функции 1: {ex.Message}";
                Function1 = null;
            }
        }

        private void BuildFunction2()
        {
            try
            {
                StatusMessage = "Построение функции 2...";

                // Попытка определить тип ввода и создать функцию
                Function2 = CreateFunctionFromInput(Function2Input);

                StatusMessage = $"Функция 2 построена. Переменных: {Function2.VariablesCount}";
                isFunction2Expanded = true; // Автоматически разворачиваются результаты
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка построения функции 2: {ex.Message}";
                Function2 = null;
            }
        }

        private void CheckEquivalence()
        {
            try
            {
                if (Function1 == null || Function2 == null)
                {
                    StatusMessage = "Ошибка: обе функции должны быть заданы";
                    return;
                }

                StatusMessage = "Проверка эквивалентности...";
                IsEquivalent = Function1.IsEquivalentTo(Function2);

                StatusMessage = IsEquivalent ?
                    "Функции эквивалентны!" :
                    "Функции не эквивалентны";
            }
            catch (Exception ex) 
            {
                StatusMessage = $"Ошибка проверки эквивалентности: {ex.Message}";
            }
        }

        #endregion

        #region Вспомогательные методы

        private LogicalFunction CreateFunctionFromInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Ввод не может быть пустым");

            input = input.Trim();

            // Попытка распарсить как число (номер функции)
            if (int.TryParse(input, out int functionNumber))
            {
                int variablesCount = DetermineVariablesCount(functionNumber);
                return LogicalFunction.FromNumber(variablesCount, functionNumber);
            }
            else
            {
                // Попытка распарсить как формулу
                return LogicalFunction.FromFormula(input);
            }
        }

        private int DetermineVariablesCount(int functionNumber)
        {
            // Простая эвристика для определения количества переменных
            if (functionNumber < 16) return 2; // 2^4 = 16 функций для 2 переменных
            if (functionNumber < 256) return 3; // 2^8 = 256 для 3 переменных
            return 4; // Для больших чисел - 4 переменные
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
