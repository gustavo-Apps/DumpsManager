using DumpManager.Data;
using DumpManager.Models;
using System.Windows;
using System.Windows.Controls;

namespace FrontPage.Pages
{
    /// <summary>
    /// Interaction logic for ListaDumps.xaml
    /// </summary>
    public partial class ListaDumps : UserControl
    {
        public ListaDumps()
        {
            InitializeComponent();
        }

        private void ListaConexoes_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new ListaConexoes();
        }

        private void SalvarNovo_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EditarCaminho_Click(object sender, RoutedEventArgs e)
        {
            if (dgConexoes.SelectedItem is Conexao conexaoSelecionada)
            {
                ((MainWindow)Application.Current.MainWindow).MainContent.Content = new EditarConexoes(conexaoSelecionada);
            }
        }

        private void ExcluirCaminho_Click(object sender, RoutedEventArgs e)
        {
            if (dgConexoes.SelectedItem is Conexao conexaoSelecionada)
            {
                var resultado = MessageBox.Show(
                    $"Tem certeza que deseja excluir a conexão \"{conexaoSelecionada.Nome}\"?",
                    "Confirmar exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (resultado == MessageBoxResult.Yes)
                {
                    var repositorio = new RepositorioConexoes();
                    repositorio.ExcluirConexao(conexaoSelecionada.Id);
                    // CarregarConexoes(); // Atualiza a grid
                }
            }
        }
    }
}