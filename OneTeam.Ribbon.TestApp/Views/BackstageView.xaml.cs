using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon.TestApp.Views
{
    public sealed partial class BackstageView : Page
    {
        public BackstageView()
        {
            InitializeComponent();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
