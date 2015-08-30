﻿using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace CustomLiveTiles
{
    public partial class LiveTileType1 : UserControl
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; txtMessage.Text = Message; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; txtTitle.Text = Title; }
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

        private Color background;

        new public Color Background
        {
            get { return background; }
            set { background = value; grdContainer.Background = new SolidColorBrush(Background); }
        }

        private int faceSelected = 1;

        public LiveTileType1()
        {
            InitializeComponent();

            liveTileAnimTop1_Part1.Completed+=liveTileAnimTop1_Part1_Completed;
            liveTileAnimTop1_Part2.Completed+=liveTileAnimTop1_Part2_Completed;
            liveTileAnimTop2_Part1.Completed+=liveTileAnimTop2_Part1_Completed;
            liveTileAnimTop2_Part2.Completed+=liveTileAnimTop2_Part2_Completed;
            CheckForAnimation();
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