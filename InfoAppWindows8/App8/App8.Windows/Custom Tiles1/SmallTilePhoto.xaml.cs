using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace CustomLiveTiles
{
    public partial class SmallTilePhoto : UserControl
    {
        private int faceSelected = 1;

        public SmallTilePhoto()
        {
            InitializeComponent();

            liveTileAnimTop1_Part2.Duration = new Duration(TimeSpan.FromSeconds(new Random().Next(2, 7)));
            liveTileAnimTop2_Part2.Duration = new Duration(TimeSpan.FromSeconds(new Random().Next(2, 7)));
            CheckForAnimation();

            SetRandomColorBlueBackground();
        }

        private void SetRandomColorBlueBackground()
        {
            Random randomColor = new Random();
            int color = randomColor.Next(0, 3);
            switch (color)
            {
                case 0:
                    LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 17, 51, 204));
                    break;

                case 1:
                    LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 17, 119, 204));
                    break;

                default:
                    LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 17, 85, 204));
                    break;
            }
        }

        
        public static DependencyProperty ImagePhotoProperty =
       DependencyProperty.Register("ImagePhoto", typeof(string), typeof(SmallTilePhoto), new PropertyMetadata(null));

        public string ImagePhoto
        {
            get
            {
                return (string)GetValue(ImagePhotoProperty);
            }
            set
            {
                SetValue(ImagePhotoProperty, value);

                imgPhoto.Source = new BitmapImage(new Uri(ImagePhoto, UriKind.Relative));
            }
        }

        private void liveTileAnimTop1_Part1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnimTop1_Part2");
            anim.Begin();
        }

        private void liveTileAnimTop2_Part1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnimTop2_Part2");
            anim.Begin();
        }

        private void liveTileAnimTop1_Part2_Completed(object sender, object e)
        {
            CheckForAnimation();
        }

        private void liveTileAnimTop2_Part2_Completed(object sender, object e)
        {
            CheckForAnimation();
        }

        private void CheckForAnimation()
        {
            if (faceSelected == 1)
            {
                SetRandomColorBlueBackground();
                faceSelected = 2;
                Storyboard anim = (Storyboard)FindName("liveTileAnimTop1_Part1");
                anim.Begin();
            }
            else if (faceSelected == 2)
            {
                faceSelected = 1;
                Storyboard anim = (Storyboard)FindName("liveTileAnimTop2_Part1");
                anim.Begin();
            }
        }
    }
}