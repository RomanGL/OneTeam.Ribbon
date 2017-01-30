using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public sealed class RibbonTitleBar : Control
    {
        private Grid titleBar, leftMask, rightMask;

        public RibbonTitleBar()
        {
            DefaultStyleKey = typeof(RibbonTitleBar);

            Window.Current.Activated += Current_Activated;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(RibbonTitleBar), new PropertyMetadata(null));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            titleBar = (Grid)GetTemplateChild(nameof(titleBar));
            leftMask = (Grid)GetTemplateChild(nameof(leftMask));
            rightMask = (Grid)GetTemplateChild(nameof(rightMask));
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            titleBar.Opacity = e.WindowActivationState != CoreWindowActivationState.Deactivated ? 1 : 0.5;
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            titleBar.Height = sender.Height;
            leftMask.Width = sender.SystemOverlayLeftInset;
            rightMask.Width = sender.SystemOverlayRightInset;
        }
    }
}
