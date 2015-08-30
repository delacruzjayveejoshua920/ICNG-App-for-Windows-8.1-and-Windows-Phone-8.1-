using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CustomLiveTiles
{
    public partial class LiveTileType5 : UserControl
    {
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

        public LiveTileType5()
        {
            InitializeComponent();
        }
    }
}