using FrontPage.Data;
using FrontPage.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FrontPage.Pages
{
    public partial class ListaConexoes : UserControl
    {
        private ObservableCollection<Conexao> conexoes = new();
        private List<string> arquivosDumps = new();

        public ListaConexoes()
        {
            InitializeComponent();
            CarregarConexoes();
            
        }

        private void CarregarConexoes()
        {
            var repo = new RepositorioConexoes();
            conexoes = new ObservableCollection<Conexao>(repo.ListarConexoes());
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
            if (dgConexoes.SelectedItem is not Conexao conexao)
            {
                MessageBox.Show("Selecione uma conexão para executar os dumps.");
                return;
            }

            foreach (var arquivo in arquivosDumps)
            {
                // Aqui vai a lógica para executar o SQL no MySQL
                MessageBox.Show($"(Simulado) Executando dump:\n{Path.GetFileName(arquivo)} na conexão {conexao.Nome}");
            }
        }

        private void ExecutarConexao_Click(object sender, RoutedEventArgs e)
        {
            if (dgConexoes.SelectedItem is Conexao conexao)
            {
                MessageBox.Show($"Executar testes ou comandos com a conexão: {conexao.Nome}");
            }
        }

        private void EditarConexao_Click(object sender, RoutedEventArgs e)
        {
            if (dgConexoes.SelectedItem is Conexao conexao)
            {
                MessageBox.Show($"Abrir tela de edição para: {conexao.Nome}");
            }
        }
    }
}
