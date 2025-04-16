using FrontPage.Models;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace FrontPage.Data
{
    public class RepositorioConexoes
    {
        private readonly string connectionString =
            $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conexoes.sqlite")}";
        public RepositorioConexoes()
        {
            CriarTabelaSeNaoExistir();
        }

        private void CriarTabelaSeNaoExistir()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Conexoes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Servidor TEXT NOT NULL,
                    Banco TEXT NOT NULL,
                    Porta TEXT NOT NULL,
                    Usuario TEXT,
                    Senha TEXT,
                    Tipo TEXT NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }

        public void SalvarConexao(Conexao conexao)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com o MySQL:\n{ex.Message}");
            }

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Conexoes (Nome, Servidor, Banco, Porta, Usuario, Senha, Tipo)
                VALUES ($nome, $servidor, $banco, $porta, $usuario, $senha, $tipo);";
            cmd.Parameters.AddWithValue("$nome", conexao.Nome);
            cmd.Parameters.AddWithValue("$servidor", conexao.Servidor);
            cmd.Parameters.AddWithValue("$banco", conexao.Banco);
            cmd.Parameters.AddWithValue("$porta", conexao.Porta);
            cmd.Parameters.AddWithValue("$usuario", conexao.Usuario);
            cmd.Parameters.AddWithValue("$senha", conexao.Senha);
            cmd.Parameters.AddWithValue("$tipo", conexao.Tipo);
            cmd.ExecuteNonQuery();
        }

        public void TestarConexao(Conexao conexao, string txtTestConnection)
        {
            try
            {
                string connectionString = "";
                connectionString = txtTestConnection;
                using var connection = new MySqlConnection(connectionString);
                    try
                    {
                        connection.Open();
                        MessageBox.Show("Conexão bem-sucedida com o MySQL!");
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao conectar com o MySQL:\n{ex.Message}");
                    }
                }

            catch (MySqlException ex)
            {
                string mensagem = "";

                switch (ex.Number)
                {
                    case 0:
                        mensagem = "Não foi possível conectar ao servidor.";
                        break;
                    case 1045:
                        mensagem = "Usuário ou senha inválidos.";
                        break;
                    case 1042:
                        mensagem = "Servidor não encontrado ou não acessível.";
                        break;
                    case 1049:
                        mensagem = "Banco de dados não existe.";
                        break;
                    default:
                        mensagem = $"Erro {ex.Number}: {ex.Message}";
                        break;
                }

                MessageBox.Show(mensagem, "Erro de Conexão",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public List<Conexao> ListarConexoes()
        {
            var lista = new List<Conexao>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Conexoes";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Conexao
                {
                    Nome = reader["Nome"].ToString(),
                    Servidor = reader["Servidor"].ToString(),
                    Banco = reader["Banco"].ToString(),
                    Porta = reader["Porta"].ToString(),
                    Usuario = reader["Usuario"].ToString(),
                    Senha = reader["Senha"].ToString(),
                    Tipo = reader["Tipo"].ToString(),
                });
            }

            return lista;
        }
    }
}