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
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window
    {
        private static readonly string LEVEL_TEXT = "Level: ";
        private List<Player> resultList = new List<Player>();
        public Results(List<Player> resultList, Level level)
        {
            InitializeComponent();
            this.resultList = resultList;
            resultsLV.ItemsSource = resultList;
            LevelTB.Text = LEVEL_TEXT + level.ToString();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
