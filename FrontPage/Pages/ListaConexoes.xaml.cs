using FrontPage.Data;
using FrontPage.Models;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
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
        private ExecutarDump executarDumpWindow;

        #region carregar e listar conexões na page

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

        #endregion carregar e listar conexões na page

        #region Carregar e Executar dumps

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

        private async void ExecutarDump_Click(object sender, RoutedEventArgs e)
        {
            btnExecutarDumps.IsEnabled = false;
            logProgressBar.Visibility = Visibility.Visible;

            try
            {
                var conexoesSelecionadas = conexoes.Where(c => c.Selecionado).ToList();
                var arquivos = lbDumps.Items.Cast<string>().ToList();

                if (!ValidarExecucao(conexoesSelecionadas, arquivos))
                    return;
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"dump_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using var logWriter = new StreamWriter(logPath, append: true);
                int total = conexoesSelecionadas.Count;
                int atual = 0;

                foreach (var conexao in conexoesSelecionadas)
                {
                    atual++;
                    AtualizarProgresso(conexao.Nome, atual, total);

                    try
                    {
                        await Task.Run(() =>
                        {
                            repositorio.ExecutarDumps(arquivos, conexao, logWriter, AddLog);

                            // Abrir a nova janela de execução
                        });
                    }
                    catch (Exception ex)
                    {
                        AddLog($"[ERRO GRAVE] {ex.Message}");
                        logWriter.WriteLine($"[ERRO GRAVE] {ex}");
                    }
                }

                // 👉 Evita janelas duplicadas
                if (executarDumpWindow == null || !executarDumpWindow.IsLoaded)
                {
                    executarDumpWindow = new ExecutarDump(conexoesSelecionadas, arquivos);
                    executarDumpWindow.Show();
                }
                else
                {
                    executarDumpWindow.Recarregar(conexoesSelecionadas, arquivos);
                    executarDumpWindow.Focus(); // ou: executarDumpWindow.Activate();
                }
            }
            finally
            {
                txtLogContainer.Visibility = Visibility.Visible;
                btnExecutarDumps.IsEnabled = true;
            }
        }

        #endregion Carregar e Executar dumps

        #region Valida se possui Conexão e Dump selecionado

        private bool ValidarExecucao(List<Conexao> conexoesSelecionadas, List<string> dumpsSelecionados)
        {
            if (conexoesSelecionadas.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos uma conexão.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (dumpsSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos um arquivo de dump.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        #endregion Valida se possui Conexão e Dump selecionado

        #region Metodo para Editar a conexão com btnEditar

        private void EditarConexao_Click(object sender, RoutedEventArgs e)
        {
            if (dgConexoes.SelectedItem is Conexao conexaoSelecionada)
            {
                ((MainWindow)Application.Current.MainWindow).MainContent.Content = new EditarConexoes(conexaoSelecionada);
            }
        }

        #endregion Metodo para Editar a conexão com btnEditar

        #region Método para Excluir Conexão

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

        #endregion Método para Excluir Conexão

        #region Métodos de Atualização de UI e Logs

        private void lnkLogPath_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_ultimoLogPath) && File.Exists(_ultimoLogPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _ultimoLogPath,
                        UseShellExecute = true // Abre com o programa padrão
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao abrir o log:\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Arquivo de log não encontrado.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AtualizarProgresso(string conexaoNome, int atual, int total)
        {
            Dispatcher.Invoke(() =>
            {
                txtProgressInfo.Text = $"{atual}/{total} - {conexaoNome}";
                pbProgress.Value = atual * 100 / total;
            });
        }

        private void AddLog(string mensagem)
        {
            Dispatcher.Invoke(() =>
            {
                lbLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {mensagem}");
                lbLog.ScrollIntoView(lbLog.Items[^1]); // Rola para o último item
            });
        }

        #endregion Métodos de Atualização de UI e Logs
    }
}