using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfLab4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создание сервисов
            var logicService = new LogicalFunctionService();
            var mainViewModel = new MainViewModel(logicService);

            // Создание глвного окна
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
        }
    }

}
