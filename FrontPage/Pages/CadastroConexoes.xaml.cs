using System.IO;
using System.Windows;
using System.Windows.Controls;
using FrontPage.Data;
using FrontPage.Models;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using SQLitePCL;

namespace FrontPage.Pages
{
    public partial class CadastroConexoes : UserControl
    {
        private readonly RepositorioConexoes repositorio = new();

        public string? connectionString { get; }
        public CadastroConexoes()
        {
            InitializeComponent();
        }

        public void btnSalvarConexao_Click(object sender, RoutedEventArgs e)
        {
            var conexao = new Conexao
            {
                Nome = txtNome.Text,
                Servidor = txtServidor.Text,
                Banco = txtBanco.Text,
                Porta = txtPorta.Text,
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Password,
                Tipo = Tipo.Text,
            };
            if (txtNome.Text == string.Empty || txtServidor.Text == string.Empty || txtBanco.Text == string.Empty || txtUsuario.Text == string.Empty)
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }
            string porta = string.IsNullOrWhiteSpace(txtPorta.Text) ? "3306" : txtPorta.Text;
            repositorio.SalvarConexao(conexao);
            MessageBox.Show("Conexão salva com sucesso!");
        }

        public void btnTestarConexao_Click(object sender, RoutedEventArgs e)
        {
            if (txtNome.Text == string.Empty || txtServidor.Text == string.Empty || txtBanco.Text == string.Empty || txtUsuario.Text == string.Empty)
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }
            var conexao = new Conexao
            {
                Nome = txtNome.Text,
                Servidor = txtServidor.Text,
                Banco = txtBanco.Text,
                Porta = txtPorta.Text,
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Password,
                Tipo = Tipo.Text,
            };
            string porta = string.IsNullOrWhiteSpace(txtPorta.Text) ? "3306" : txtPorta.Text;
            string tipo = string.IsNullOrWhiteSpace(Tipo.Text) ? "mariadb" : Tipo.Text;
            string connStr = $"Server={txtServidor.Text};Port={txtPorta.Text};Database={txtBanco.Text};Uid={txtUsuario.Text};Pwd={txtSenha.Password};";

            try
            {
                using var mysqlConnection = new MySqlConnection(connStr);
                mysqlConnection.Open();

                var resultado = MessageBox.Show("Conexão bem-sucedida com o MySQL!\nDeseja salvar a conexão?",
                    "Salvar conexão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    repositorio.SalvarConexao(conexao);
                    MessageBox.Show("Conexão salva com sucesso!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com o MySQL:\n{ex.Message}");
            }

        }
    }
}