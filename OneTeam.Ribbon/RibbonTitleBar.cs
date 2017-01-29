using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public sealed class RibbonTitleBar : Control
    {
        public RibbonTitleBar()
        {
            DefaultStyleKey = typeof(RibbonTitleBar);
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(RibbonTitleBar), new PropertyMetadata(null));
    }
}
