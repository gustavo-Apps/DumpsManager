using MySql.Data.MySqlClient;
using System.ComponentModel;

namespace DumpManager.Models
{
    public class Conexao : INotifyPropertyChanged
    {
        #region Metodos para coletar as conexoes

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Servidor { get; set; } = string.Empty;
        public string Banco { get; set; } = string.Empty;
        public string Porta { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;

        private bool _selecionado;

        #endregion Metodos para coletar as conexoes

        #region Construtor para conectar-se as bases

        public MySqlConnection CriarConexao()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = Servidor,
                Database = Banco,
                UserID = Usuario,
                Password = Senha,
            };
            if (uint.TryParse(Porta, out uint portaConvertida))
            {
                builder.Port = portaConvertida;
            }
            else
            {
                throw new FormatException($"Porta inválida: '{Porta}'");
            }

            return new MySqlConnection(builder.ConnectionString);
        }

        #endregion Construtor para conectar-se as bases

        public bool Selecionado
        {
            get => _selecionado;
            set
            {
                if (_selecionado != value)
                {
                    _selecionado = value;
                    OnPropertyChanged(nameof(Selecionado)); // Notifica a UI da mudança
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}