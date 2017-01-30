using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace OneTeam.Ribbon
{
    public sealed class Ribbon : ItemsControl
    {
        private Rectangle backgroundElement;
        private ListView headersListView;

        public Ribbon()
        {
            DefaultStyleKey = typeof(Ribbon);
        }

        public static readonly new DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(nameof(Background),
            typeof(SolidColorBrush), typeof(Ribbon), new PropertyMetadata(null, OnBackgroundPropertyChanged));

        public static readonly new DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(nameof(Foreground),
            typeof(SolidColorBrush), typeof(Ribbon), new PropertyMetadata(null, OnForegroundPropertyChanged));

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

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Ribbon), new PropertyMetadata(null));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            backgroundElement = (Rectangle)GetTemplateChild(nameof(backgroundElement));
            Window.Current.SetTitleBar(backgroundElement);

            headersListView = (ListView)GetTemplateChild(nameof(headersListView));
            headersListView.ItemsSource = Items;
            headersListView.ItemClick += HeadersListView_ItemClick;
        }

        private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = titleBar.InactiveBackgroundColor =
                titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = ((SolidColorBrush)e.NewValue).Color;
        }

        private static void OnForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ForegroundColor = titleBar.ButtonForegroundColor =
                titleBar.ButtonHoverForegroundColor = titleBar.ButtonPressedForegroundColor =
                    titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor = ((SolidColorBrush)e.NewValue).Color;
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
    }
}
