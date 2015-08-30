using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace CustomLiveTiles
{
    public partial class LiveTileType3 : UserControl
    {
        private Color background;

        new public Color Background
        {
            get
            {
                return background;
            }
            set
            {
                background = value;
                SolidColorBrush brush = new SolidColorBrush(Background);
                grdContainer.Background = brush;
            }
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                txtTitle.Text = Title;
            }
        }

        private ImageSource source;

        public ImageSource Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                imgBackground.Source = Source;
            }
        }

        public LiveTileType3()
        {
            InitializeComponent();
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Part1");
            liveTileAnim1_Part1.Completed += liveTileAnim1_Part1_Completed;
            liveTileAnim1_Part2.Completed+=liveTileAnim1_Part2_Completed;
            liveTileAnim2_Part1.Completed+=liveTileAnim2_Part1_Completed;
            liveTileAnim2_Part2.Completed+=liveTileAnim2_Part2_Completed;
            anim.Begin();
        }

        private void liveTileAnim1_Part1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Part2");
            anim.Begin();
        }

        private void liveTileAnim1_Part2_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim2_Part1");
            anim.Begin();
        }

        private void liveTileAnim2_Part1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim2_Part2");
            anim.Begin();
        }

        private void liveTileAnim2_Part2_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Part1");
            anim.Begin();
        }
    }
}