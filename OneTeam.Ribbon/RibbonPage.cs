using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public partial class RibbonPage : Page
    {
        public RibbonPage()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }
    }
}
