using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public sealed class RibbonTitleBar : Control
    {
        private Grid titleBar;
        private Grid leftMask;
        private Grid rightMask;

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
            DependencyProperty.Register(nameof(Title), typeof(string), 
                typeof(RibbonTitleBar), new PropertyMetadata(default(string)));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            titleBar = GetTemplateChild("titleBar") as Grid;
            leftMask = GetTemplateChild("leftMask") as Grid;
            rightMask = GetTemplateChild("rightMask") as Grid;
            
            if (!DesignMode.DesignModeEnabled)
            {
                Window.Current.Activated += Current_Activated;
                CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            }
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
