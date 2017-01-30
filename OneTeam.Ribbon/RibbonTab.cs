using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public sealed class RibbonTab : ContentControl
    {
        public RibbonTab()
        {
            DefaultStyleKey = typeof(RibbonTab);
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), 
                typeof(RibbonTab), new PropertyMetadata(null));
    }
}
