using FrontPage.Data;
using FrontPage.Models;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace FrontPage.Pages
{
    /// <summary>
    /// Interaction logic for EditarConexoes.xaml
    /// </summary>
    public partial class EditarConexoes : UserControl
    {
        private Conexao conexaoAtual;
        private readonly RepositorioConexoes repo = new();

        public EditarConexoes(Conexao conexao)
        {
            InitializeComponent();
            conexaoAtual = conexao;
            PreencherCampos();
        }

        private void PreencherCampos()
        {
            txtNome.Text = conexaoAtual.Nome;
            txtServidor.Text = conexaoAtual.Servidor;
            txtBanco.Text = conexaoAtual.Banco;
            txtPorta.Text = conexaoAtual.Porta;
            txtUsuario.Text = conexaoAtual.Usuario;
            txtSenha.Password = conexaoAtual.Senha;
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            conexaoAtual.Nome = txtNome.Text;
            conexaoAtual.Servidor = txtServidor.Text;
            conexaoAtual.Banco = txtBanco.Text;
            conexaoAtual.Porta = txtPorta.Text;
            conexaoAtual.Usuario = txtUsuario.Text;
            conexaoAtual.Senha = txtSenha.Password;

            try
            {
                string connStr = $"Server={txtServidor.Text};Port={txtPorta.Text};Database={txtBanco.Text};Uid={txtUsuario.Text};Pwd={txtSenha.Password};";
                using var connection = new MySqlConnection(connStr);
                connection.Open();
                MessageBox.Show("Conexão bem-sucedida!");

                if (connection is not null)
                {
                    repo.EditarConexao(conexaoAtual);
                    MessageBox.Show("Conexão atualizada!\nVoltando a listagem");
                    ((MainWindow)Application.Current.MainWindow).MainContent.Content = new ListaConexoes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnTestar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connStr = $"Server={txtServidor.Text};Port={txtPorta.Text};Database={txtBanco.Text};Uid={txtUsuario.Text};Pwd={txtSenha.Password};";
                using var connection = new MySqlConnection(connStr);
                connection.Open();
                MessageBox.Show("Conexão bem-sucedida!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Voltar para a lista/dash
            ((MainWindow)Application.Current.MainWindow).MainContent.Content = new ListaConexoes();
        }
    }
}