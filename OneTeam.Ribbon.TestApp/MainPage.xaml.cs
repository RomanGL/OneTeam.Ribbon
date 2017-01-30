using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OneTeam.Ribbon.TestApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ChangeBackgroundButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var color = ((SolidColorBrush)ribbon.Background).Color;
            color.R += 20;
            color.G += 5;
            color.B -= 10;

            ((SolidColorBrush)ribbon.Background).Color = color;
        }

        private void ChangeForegroundButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var color = ((SolidColorBrush)ribbon.Foreground).Color;
            color.R += 20;
            color.G += 5;
            color.B -= 10;

            ((SolidColorBrush)ribbon.Foreground).Color = color;
        }
    }
}
