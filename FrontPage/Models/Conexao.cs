using System.ComponentModel;

namespace FrontPage.Models
{
    public class Conexao : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Servidor { get; set; } = string.Empty;
        public string Banco { get; set; } = string.Empty;
        public string Porta { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        private bool _selecionado;

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