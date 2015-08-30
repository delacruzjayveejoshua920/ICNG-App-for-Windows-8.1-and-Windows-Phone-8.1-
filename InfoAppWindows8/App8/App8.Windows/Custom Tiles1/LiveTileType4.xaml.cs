using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace CustomLiveTiles
{
    public partial class LiveTileType4 : UserControl
    {
        public LiveTileType4()
        {
            InitializeComponent();

            liveTileAnimTop.Completed += liveTileAnimTop_Completed;
            liveTileAnimBottom.Completed += liveTileAnimBottom_Completed;
            Storyboard anim = (Storyboard)FindName("liveTileAnimTop");
            anim.Begin();
        }

        void liveTileAnimBottom_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnimTop");
            anim.Begin();
        }

        void liveTileAnimTop_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnimBottom");
            anim.Begin();
        }
    }
}