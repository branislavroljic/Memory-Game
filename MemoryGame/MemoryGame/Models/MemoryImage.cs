using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MemoryGame.Models
{
    public class MemoryImage : Image
    {
        private BitmapImage hiddenImage;
        private BitmapImage showingImage;

        public Storyboard firstHalfStoryBoard { get; set; }
        public Storyboard secondHalfStoryBoard { get; set; }
        public int Id { get; }

        public MemoryImage(string name, int Id, BitmapImage showingImage)
        {
            this.Id = Id;
            this.showingImage = showingImage;
            this.hiddenImage = GameWindow.ByteToBitmapImage(ImagesResources.question);
            this.Stretch = Stretch.Uniform;
            Name = name + Id;
            RenderTransformOrigin = new Point(0.5, 0.5);
            RenderTransform = new ScaleTransform();
            Source = hiddenImage;
            showing = false;
            Matched = false;
            Width = 100;
            Height = 100;
        }



        private bool showing;

        public bool Showing
        {
            get { return showing; }
            set
            {
                showing = value;
                if (showing)
                    this.Source = showingImage;
                else this.Source = hiddenImage;
            }
        }

        private bool matched;

        public bool Matched
        {
            get { return matched; }
            set
            {
                matched = value;
                if (matched)
                    Source = showingImage;
            }
        }



        public MemoryImage Clone()
        {
            return new MemoryImage(this.Name + "clone", this.Id, this.showingImage);
        }

    }
}
