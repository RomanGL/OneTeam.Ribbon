using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneTeam.Ribbon
{
    public class Ribbon : ItemsControl
    {
        public Ribbon()
        {
            DefaultStyleKey = typeof(Ribbon);
        }
        
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(Ribbon), new PropertyMetadata(-1, OnSelectedIndexChanged));
        
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(Ribbon), new PropertyMetadata(null));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _headersListView = GetTemplateChild("HeadersListView") as ListView;

            _headersListView.ItemsSource = Items;
            _headersListView.ItemClick += HeadersListView_ItemClick;
        }

        private void HeadersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedIndex = Items.IndexOf(e.ClickedItem);
        }

        private static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            int newIndex = (int)e.NewValue;
            if (newIndex < 0)
                return;

            var ribbon = (Ribbon)obj;
            ribbon.SelectedItem = ribbon.Items[newIndex];
        }

        private ListView _headersListView;
    }
}
