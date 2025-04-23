using FrontPage.Data;
using FrontPage.Pages;
using System.Windows;
using System.Windows.Controls;

namespace FrontPage
{
    public partial class MainWindow : Window
    {
        private readonly RepositorioConexoes repositorio = new();
        public string? connectionString { get; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public ContentControl MainContentControl => MainContent;

        private void BtnConexoes_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ListaConexoes(); // insere a tela dentro do painel da direita
            //var janela = new FrontPage.Pages.ListaConexoes(); // namespace + nome da janela
            //janela.Show(); // Mostra a janela        }
        }

        private void BtnDumps_Click(object sender, RoutedEventArgs e)
        {
            // Exemplo: criar outra página depois
            MessageBox.Show("Página de dumps em construção!");
            MainContent.Content = new ListaDumps(); // insere a tela dentro do painel da direita
        }

        private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CadastroConexoes(); // insere a tela dentro do painel da direita
        }
    }
}