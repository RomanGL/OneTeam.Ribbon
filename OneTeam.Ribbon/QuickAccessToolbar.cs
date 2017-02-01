using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public class QuickAccessToolbar : ItemsControl
    {
        public QuickAccessToolbar()
        {
            DefaultStyleKey = typeof(QuickAccessToolbar);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
