using FrontPage.Data;
using FrontPage.Models;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows;

namespace FrontPage.Pages
{
    public partial class ExecutarDump : Window
    {
        private readonly List<Conexao> conexoes;
        private readonly List<string> dumps;
        private readonly string logPath;
        private readonly RepositorioConexoes repositorio = new();

        public ExecutarDump(List<Conexao> conexoesSelecionadas, List<string> dumpsSelecionados)
        {
            InitializeComponent();
            conexoes = conexoesSelecionadas;
            dumps = dumpsSelecionados;
            logPath = GerarCaminhoLog();
            _ = ExecutarDumpsAsync(); // inicia sem await para não travar a UI
        }

        private async Task ExecutarDumpsAsync()
        {
            using var logWriter = new StreamWriter(logPath, append: true);
            int totalBases = conexoes.Count;
            int baseAtual = 0;

            AddLog("▶️ Iniciando execução dos dumps...");

            foreach (var conexao in conexoes)
            {
                baseAtual++;

                Dispatcher.Invoke(() =>
                {
                    txtCurrentConnection.Text = conexao.Nome;
                    txtProgressInfo.Text = $"{baseAtual}/{totalBases}";
                    pbProgress.Value = baseAtual * 100 / totalBases;
                });

                try
                {
                    await Task.Run(() =>
                    {
                        using var connection = conexao.CriarConexao();
                        connection.Open();

                        foreach (var arquivo in dumps)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                txtArquivoAtual.Text = Path.GetFileName(arquivo);
                            });

                            AddLog($"📄 Processando: {Path.GetFileName(arquivo)}");

                            string conteudo = File.ReadAllText(arquivo);
                            var comandos = conteudo.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            int totalComandos = comandos.Count(c => !string.IsNullOrWhiteSpace(c));
                            int comandosExecutados = 0;

                            foreach (var comando in comandos)
                            {
                                string trimmed = comando.Trim();
                                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                                comandosExecutados++;

                                Dispatcher.Invoke(() =>
                                {
                                    txtComandoCount.Text = $"{comandosExecutados}/{totalComandos}";
                                });

                                try
                                {
                                    using var cmd = new MySqlCommand(trimmed, connection);
                                    cmd.ExecuteNonQuery();

                                    string msg = $"[{conexao.Nome}] ✅ {TrimComando(trimmed)}";
                                    AddLog(msg);
                                    logWriter.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}");
                                }
                                catch (Exception ex)
                                {
                                    string erro = $"[{conexao.Nome}] ❌ {TrimComando(trimmed)} - {ex.Message}";
                                    AddLog(erro);
                                    logWriter.WriteLine($"[{DateTime.Now:HH:mm:ss}] {erro}");
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    AddLog($"[ERRO GRAVE] {ex.Message}");
                    logWriter.WriteLine($"[ERRO GRAVE] {ex}");
                }
            }

            AddLog("🏁 Execução concluída com sucesso!");
            MessageBox.Show("Execução concluída!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddLog(string mensagem)
        {
            Dispatcher.Invoke(() =>
            {
                lbLog.Items.Add($"[{DateTime.Now:HH:mm:ss}] {mensagem}");
                lbLog.ScrollIntoView(lbLog.Items[lbLog.Items.Count - 1]);
            });
        }

        private string TrimComando(string sql)
        {
            return sql.Length > 50 ? sql.Substring(0, 50) + "..." : sql.Replace("\r", "").Replace("\n", " ");
        }

        private string GerarCaminhoLog()
        {
            string pastaLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(pastaLogs);
            return Path.Combine(pastaLogs, $"dump_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        }

        private void btnParar_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Deseja realmente sair da execução?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        public void Recarregar(List<Conexao> novasConexoes, List<string> novosDumps)
        {
            conexoes.AddRange(novasConexoes);
            dumps.AddRange(novosDumps);
            lbLog.Items.Clear();
            _ = ExecutarDumpsAsync();
        }
    }
}