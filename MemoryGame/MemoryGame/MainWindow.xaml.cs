using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace MemoryGame
{

    public enum ImageCategory
    {
        Animals,
        Cars
    }
    public enum Level
    {
        Easy = 2,
        Medium = 4,
        Hard = 6
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Button> levelButtons = new List<Button>();


        Button selectedLevel;
        public static readonly string RESULTS_PATH = "results.txt";

        private static readonly double OPACITY = 0.5;
        public MainWindow()
        {
            InitializeComponent();

            levelButtons.Add(Easy);
            levelButtons.Add(Medium);
            levelButtons.Add(Hard);
            selectedLevel = Medium;



        }


        private Storyboard animate(MemoryImage memoryImage, Double to)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = to;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            // myDoubleAnimation.EasingFunction = 


            Storyboard myStoryboard2 = new Storyboard();
            myStoryboard2.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetName(myDoubleAnimation, memoryImage.Name);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("RenderTransform.ScaleX"));

            return myStoryboard2;
        }
        private async void Level_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Opacity = 1.0;
            string name = button.Name;
            levelButtons.ForEach(b =>
            {
                if (!name.Equals(b.Name))
                {
                    b.Opacity = OPACITY;
                }

            });
            selectedLevel = button;

        }
        private async void Start_Click(object sender, RoutedEventArgs e)
        {

            Button categoryButton = (Button)sender;
            Level level;
            ImageCategory category;
            if (Enum.TryParse(categoryButton.Name, out category) && Enum.TryParse(selectedLevel.Name, out level))
            {

                new GameWindow(level, category).Show();
                this.Close();
            }
        }

        private async void Results_Click(object sender, RoutedEventArgs e)
        {

            if (Enum.TryParse(selectedLevel.Name, out Level currLevel))
            {
                List<Player> resultsList = new List<Player>();
                var results = await ReadResultsAsync();
                Level level;
                foreach (string result in results)
                {
                    if (string.IsNullOrEmpty(result))
                        break;

                    level = (Level)int.Parse(result.Split("#")[2]);

                    if ((int)level == (int)currLevel)
                        resultsList.Add(new Player(result.Split('#')[0], GetScore(level, int.Parse(result.Split('#')[1])), level));
                }

               resultsList =  resultsList.OrderByDescending(p => p.Score).ToList();

                this.Effect = new BlurEffect();
                new Results(resultsList, currLevel).ShowDialog();
                this.Effect = null;
            }
        }

        public static int GetScore(Level level, int res)
        {
            switch (level)
            {
                case Level.Easy:
                    return (int)((double)1 / res * 100);
                case Level.Medium:
                    return (int)((double)1 / res * 1000);
                case Level.Hard:
                    return (int)((double)1 / res * 10000);

                default:  return 0;
            }
        }
        public static async Task<string[]> ReadResultsAsync()
        {
            using (var reader = File.OpenText("results.txt"))
            {
                var fileText = await reader.ReadToEndAsync();
                return fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
