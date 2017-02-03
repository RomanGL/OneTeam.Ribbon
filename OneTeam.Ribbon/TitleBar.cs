using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public sealed class TitleBar : Control
    {
        private Grid titleBar;
        private Grid leftMask;
        private Grid rightMask;
        private CoreApplicationViewTitleBar coreTitleBar;

        public TitleBar()
        {
            DefaultStyleKey = typeof(TitleBar);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            titleBar = GetTemplateChild("titleBar") as Grid;
            leftMask = GetTemplateChild("leftMask") as Grid;
            rightMask = GetTemplateChild("rightMask") as Grid;
            
            if (!DesignMode.DesignModeEnabled)
            {
                coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            }
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            titleBar.Height = sender.Height;
            leftMask.Width = sender.SystemOverlayLeftInset;
            rightMask.Width = sender.SystemOverlayRightInset;
        }
    }
}
