using FrontPage.Data;
using FrontPage.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FrontPage.Pages
{
    public partial class ListaConexoes : UserControl
    {
        private string _ultimoLogPath;
        private ObservableCollection<Conexao> conexoes = new();
        private List<string> arquivosDumps = new();
        private readonly RepositorioConexoes repositorio = new();

        public ListaConexoes()
        {
            InitializeComponent();
            CarregarConexoes();
        }

        private void CarregarConexoes()
        {
            var lista = repositorio.ListarConexoes();

            conexoes.Clear(); // Limpa mantendo o binding ativo

            foreach (var conexao in lista)
            {
                conexoes.Add(conexao); // Adiciona uma a uma
            }
            dgConexoes.ItemsSource = conexoes;
        }

        private void CarregarDumps_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "SQL Files (*.sql)|*.sql",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                arquivosDumps = new List<string>(dialog.FileNames);
                lbDumps.ItemsSource = arquivosDumps;
            }
        }

        private void ExecutarDump_Click(object sender, RoutedEventArgs e)
        {
            var conexoesSelecionadas = conexoes.Where(c => c.Selecionado).ToList();

            if (conexoesSelecionadas.Count == 0)
            {
                MessageBox.Show("Nenhuma conexão selecionada para executar os dumps.");
                return;
            }

            if (arquivosDumps.Count < 1)
            {
                MessageBox.Show("Nenhum dump adicionado.");
                return;
            }

            // Cria um log único para todas as execuções
            _ultimoLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        $"execucao_dumps_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            using (var logWriter = new StreamWriter(_ultimoLogPath, append: true))
            {
                logWriter.WriteLine($"=== Execução iniciada: {DateTime.Now} ===");

                var dumps = lbDumps.Items.Cast<string>();

                foreach (var conexao in conexoesSelecionadas)
                {
                    logWriter.WriteLine($"\n--- Executando na conexão: {conexao.Nome} ---");
                    try
                    {
                        repositorio.ExecutarDumps(dumps, conexao, logWriter);
                        logWriter.WriteLine($"[SUCESSO] Todos os dumps executados na conexão {conexao.Nome}");
                    }
                    catch (Exception ex)
                    {
                        logWriter.WriteLine($"[ERRO] Falha na conexão {conexao.Nome}: {ex.Message}");
                    }
                }

                logWriter.WriteLine($"\n=== Execução finalizada: {DateTime.Now} ===");
            }

            // Atualiza o hyperlink (assumindo que você tem um TextBlock chamado txtLogInfo)
            txtLogContainer.Visibility = Visibility.Visible;
            txtLogPath.Text = _ultimoLogPath; // Mostra apenas o caminho como link

            MessageBox.Show("Execução em todas as conexões finalizada!");
        }

        private void EditarConexao_Click(object sender, RoutedEventArgs e)
        {
            if (dgConexoes.SelectedItem is Conexao conexaoSelecionada)
            {
                ((MainWindow)Application.Current.MainWindow).MainContent.Content = new EditarConexoes(conexaoSelecionada);
            }
        }

        private void ExcluirConexao_Click(object sender, RoutedEventArgs e)
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
                    CarregarConexoes(); // Atualiza a grid
                }
            }
        }

        private void lnkLogPath_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_ultimoLogPath) && File.Exists(_ultimoLogPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _ultimoLogPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao abrir o log:\n{ex.Message}");
                }
            }
        }
    }
}