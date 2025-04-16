using SQLitePCL;
using System.Windows;

namespace FrontPage
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Batteries.Init(); // Isso é o suficiente para configurar SQLite
            InitializeComponent();
        }
    }
}