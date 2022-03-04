using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MemoryGame
{


    enum FlipState
    {
        NoneFlipped,
        OneFlipped,
        ThowFlipped
    }

    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window, INotifyPropertyChanged
    {

        private int numOfRows;
        private int numOfColumns;
        private List<MemoryImage> memoryImages = new List<MemoryImage>();
        private ImageCategory category;
        private FlipState flipState = FlipState.NoneFlipped;

        List<MemoryImage> flippedImages = new List<MemoryImage>();

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Left = window.Left + e.HorizontalChange;
            window.Top = window.Top + e.VerticalChange;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private int numMatched = 0;

        private int numOfAttempts = 0;

        private Level level;

        public int NumOfAttempts
        {
            get { return numOfAttempts; }
            set
            {
                numOfAttempts = value;
                OnPropertyChanged(nameof(NumOfAttempts));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;


        Player player;

        // public TimerViewModel Timer { get; private set; }

       // private DispatcherTimer timer;
        private DispatcherTimer flipTimer;

        //private TimeSpan elapsedTime;

        //public TimeSpan ElapsedTime

        //{
        //    get { return elapsedTime; }
        //    set
        //    {
        //        elapsedTime = value;
        //        OnPropertyChanged(nameof(ElapsedTime));
        //    }
        //}


        public GameWindow(Level level, ImageCategory category)
        {
            InitializeComponent();

            this.level = level;

            DataContext = this;


            numOfRows = (int)level;
            numOfColumns = (int)level;

            //hardcoded
            this.category = category;

            this.player = new Player();


            //timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 1);
            //timer.Tick += Timer_Tick;
            //timer.Start();

            flipTimer = new DispatcherTimer();
            flipTimer.Interval = new TimeSpan(0, 0, 1);
            flipTimer.Tick += Flip_Timer_Tick;


            List<MemoryImage> allMemoryImages = category == ImageCategory.Cars ? LoadCarImages() : LoadAnimalImages();

            Random rand = new();
            allMemoryImages = allMemoryImages.OrderBy(a => rand.Next()).ToList();

            for (int i = 0; i < numOfColumns * numOfColumns / 2; i++)
            {
                memoryImages.Add(allMemoryImages[i]);
                MemoryImage cloneMemoryImage = allMemoryImages[i].Clone();
                memoryImages.Add(cloneMemoryImage);
            }

            memoryImages = memoryImages.OrderBy(a => rand.Next()).ToList();
            populateGrid();

        }

        DispatcherTimer timer = null;
        void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Elapsed);
            timer.Start();
        }

        void timer_Elapsed(object sender, EventArgs e)
        {
            timer.Stop();
            new FinalResultWindow(player).Show();
            this.Close();
        }


        private void Flip_Timer_Tick(object? sender, EventArgs e)
        {
            flipTimer.Stop();

            flipState = FlipState.NoneFlipped;
            flippedImages[0].firstHalfStoryBoard.Begin(this);
            flippedImages[1].firstHalfStoryBoard.Begin(this);
            flippedImages.Clear();

        }


        private void populateGrid()
        {
            var colDefinitions = MemoryGrid.ColumnDefinitions;
            var rowDefinitions = MemoryGrid.RowDefinitions;

            for (int colCount = 0; colCount < numOfColumns; colCount++)
            {
                colDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int rowCount = 0; rowCount < numOfRows; rowCount++)
            {
                rowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int rowCount = 0; rowCount < numOfRows; rowCount++)
            {
                for (int colCount = 0; colCount < numOfColumns; colCount++)

                {
                    MemoryImage memoryImage = memoryImages[numOfColumns * rowCount + colCount];

                    this.RegisterName(memoryImage.Name, memoryImage);

                    memoryImage.firstHalfStoryBoard = animate(memoryImage, 0.0);
                    memoryImage.secondHalfStoryBoard = animate(memoryImage, 1.0);

                    memoryImage.firstHalfStoryBoard.Completed += (s, e) =>
                    {
                        memoryImage.Showing = !memoryImage.Showing;
                        memoryImage.secondHalfStoryBoard.Begin(this);
                    };

                    memoryImage.MouseLeftButtonDown += On_Image_Click;

                    Border border = new Border();
                    border.Padding = new Thickness(10);
                    border.Child = memoryImage;
                    Grid.SetColumn(border, colCount);
                    Grid.SetRow(border, rowCount);

                    MemoryGrid.Children.Add(border);
                }
            }
        }

        private void FirstHalfStoryBoard_Completed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void On_Image_Click(object sender, MouseButtonEventArgs e)
        {
            var memoryImage = sender as MemoryImage;    
            if (memoryImage.Matched || flippedImages.Contains(memoryImage) || flipState == FlipState.ThowFlipped)
            {
                
                return;
            }


            flippedImages.Add(memoryImage);
            switch (flipState)
            {
                case FlipState.NoneFlipped:
                    memoryImage.firstHalfStoryBoard?.Begin(this);
                    flipState = FlipState.OneFlipped;
                    break;
                case FlipState.OneFlipped:
                    flipState = FlipState.ThowFlipped;
                    NumOfAttempts++;

                    memoryImage.firstHalfStoryBoard?.Begin(this);

                    if (flippedImages[0].Id == flippedImages[1].Id)
                    {
                        numMatched++;
                        flippedImages[0].Matched = true;
                        flippedImages[1].Matched = true;
                        flippedImages.Clear();
                        if (numMatched == numOfColumns * numOfRows / 2)
                        {
                            //timer.Stop();
                            player.Score = (int)(numOfAttempts);
                            player.Level = level;


                            //KRAJ IGRE
                            StartTimer();
                            

                        }
                        flipState = FlipState.NoneFlipped;
                        return;
                    }
                    flipTimer.Start();

                    break;
                default:
                    break;
            }


        }

        private static Storyboard animate(MemoryImage memoryImage, double to)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.To = to;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));
            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetName(myDoubleAnimation, memoryImage.Name);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("RenderTransform.ScaleX"));

            return storyBoard;
        }

        //private List<MemoryImage> loadImages(string imagesCategoryPath)
        //{
        //    List<MemoryImage> memoryImages = new List<MemoryImage>();
        //    var images = Directory.GetFiles(imagesCategoryPath);
        //    int Id = 1;
        //    foreach (var image in images)
        //    {
        //        MemoryImage memoryImage = new MemoryImage(category.ToString(), Id, new BitmapImage(new Uri(image)));

        //        memoryImages.Add(memoryImage);

        //        Id++;
        //    }
        //    return memoryImages;
        //}
        private List<MemoryImage> LoadAnimalImages()
        {
            List<byte[]> imageResources = new List<byte[]>() {
                ImagesResources.animal1,
                ImagesResources.animal2,
                ImagesResources.animal3,
                ImagesResources.animal4,
                ImagesResources.animal5,
                ImagesResources.animal6,
                ImagesResources.animal7,
                ImagesResources.animal8,
                ImagesResources.animal9,
                ImagesResources.animal10,
                ImagesResources.animal11,
                ImagesResources.animal12,
                ImagesResources.animal13,
                ImagesResources.animal14,
                ImagesResources.animal15,
                ImagesResources.animal16,
                ImagesResources.animal17,
                ImagesResources.animal18
            };

            List<MemoryImage> memoryImages = new List<MemoryImage>();
            int Id = 1;
            foreach (var image in imageResources)
            {
                MemoryImage memoryImage = new MemoryImage(category.ToString(), Id, ByteToBitmapImage(image));

                memoryImages.Add(memoryImage);

                Id++;
            }
            return memoryImages;
        }

        private List<MemoryImage> LoadCarImages()
        {
            List<byte[]> imageResources = new List<byte[]>() {
                 ImagesResources.car1,
                ImagesResources.car2,
                ImagesResources.car3,
                ImagesResources.car4,
                ImagesResources.car5,
                ImagesResources.car6,
                ImagesResources.car7,
                ImagesResources.car8,
                ImagesResources.car9,
                ImagesResources.car10,
                ImagesResources.car11,
                ImagesResources.car12,
                ImagesResources.car13,
                ImagesResources.car14,
                ImagesResources.car15,
                ImagesResources.car16,
                ImagesResources.car17,
                ImagesResources.car18
            };

            List<MemoryImage> memoryImages = new List<MemoryImage>();
            int Id = 1;
            foreach (var image in imageResources)
            {
                MemoryImage memoryImage = new MemoryImage(category.ToString(), Id, ByteToBitmapImage(image));

                memoryImages.Add(memoryImage);

                Id++;
            }
            return memoryImages;
        }

        public static BitmapImage ByteToBitmapImage(byte[] array)
            {
                using (var ms = new System.IO.MemoryStream(array))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }


        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

       
    }
}
