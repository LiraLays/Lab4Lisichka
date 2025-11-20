using System.Windows;
using System.Windows.Controls;

namespace WpfLab4
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            SetText();
        }

        private void SetText()
        {
            TextBox1.Text = "Как вводить функции";
            TextBox2.Text = "Способ 1: Формула\nA & B          (И)\nA | B          (ИЛИ)  \n" +
                "!A             (НЕ)\n(A|B)&C        (со скобками)\n" +
                "\r\nСпособ 2: Номер функции\n0-15    - 2 переменные\n16-255  - 3 переменные\n256+    - 4 переменные";
            TextBox3.Text = "\nБыстрый старт";
            TextBox4.Text = "1. Введите в поле \"Функция 1\": A & B \n2. Нажмите \"Построить функцию 1\"\n" +
                "3. Перейдите во вкладку \"Функция 1\"\n4. Смотрите результаты:\n- Таблица истинности\n" +
                "- DNF/KNF формы\n- Стоимость функции";
            TextBox5.Text = "\nПроверка эквивалентности";
            TextBox6.Text = "1. Постройте обе функции\n2. Нажмите \"Проверить эквивалентность\"\n" +
                "3. Смотрите результат:\n- Зеленый - функции одинаковые\n" +
                "- Красный - функции разные";
            TextBox7.Text = "\nВажные правила";
            TextBox8.Text = "1. Переменные: только A, B, C, D, E\n2. Операторы: & (И), | (ИЛИ), ! (НЕ)\n3. Скобки: обязательны для сложных выражений\n4. Пробелы: не важны";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}