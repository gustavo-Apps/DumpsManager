using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontPage.Models
{
   public class Conexao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Servidor { get; set; } = string.Empty;
        public string Banco { get; set; } = string.Empty;
        public string Porta { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; 
    }
}
