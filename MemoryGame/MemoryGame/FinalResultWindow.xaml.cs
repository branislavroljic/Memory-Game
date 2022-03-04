using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGame
{
    /// <summary>
    /// Interaction logic for FinalResultWindow.xaml
    /// </summary>
    public partial class FinalResultWindow : Window
    {
        public static readonly string RESULTS_PATH = "results.txt";

        public Player Player { get; }
        public FinalResultWindow(Player player)
        {
            InitializeComponent();
            this.Player = player;
            ScoreLabel.Content = MainWindow.GetScore(player.Level, player.Score);
        }

        private async void Home_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();

        }

        private async void ReturnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SaveResult();
            }
        }

        private async void SaveResult()
        {
            if (!String.IsNullOrEmpty(NameText.Text))
            {
                Player.Username = NameText.Text;
                NameText.IsReadOnly = true;
                await WriteResultToFileAsync(RESULTS_PATH, Player);

            }
            new MainWindow().Show();
            this.Close();
        }

        public static async Task WriteResultToFileAsync(string path, Player player)
        {
            using StreamWriter file = new(path, append: true);
            await file.WriteLineAsync(player.ToString());
        }
    }
}
