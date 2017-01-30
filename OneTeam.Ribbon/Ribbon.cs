using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
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

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public bool ExtendIntoTitleBar
        {
            get { return (bool)GetValue(ExtendIntoTitleBarProperty); }
            set { SetValue(ExtendIntoTitleBarProperty, value); }
        }
        
        public static readonly DependencyProperty ExtendIntoTitleBarProperty =
            DependencyProperty.Register(nameof(ExtendIntoTitleBar), typeof(bool), 
                typeof(Ribbon), new PropertyMetadata(true, OnExtendIntoTitleBarChanged));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), 
                typeof(Ribbon), new PropertyMetadata(-1, OnSelectedIndexChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), 
                typeof(Ribbon), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), 
                typeof(Ribbon), new PropertyMetadata(null));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            backgroundElement = GetTemplateChild("backgroundElement") as Rectangle;
            headersListView = GetTemplateChild("headersListView") as ListView;

            headersListView.ItemsSource = Items;
            
            if (!DesignMode.DesignModeEnabled)
            {
                Window.Current.SetTitleBar(backgroundElement);
                UpdateTitleBar();
                UpdateExtendIntoTitleBar();
                headersListView.ItemClick += HeadersListView_ItemClick;

                RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundChanged);
                Background.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBackgroundChanged);
            }
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

        private void UpdateTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            var brush = Background as SolidColorBrush;
            if (brush == null)
            {
                titleBar.BackgroundColor = null;
                titleBar.InactiveBackgroundColor = null;
                titleBar.ButtonBackgroundColor = null;
                titleBar.ButtonInactiveBackgroundColor = null;
            }
            else
            {
                titleBar.BackgroundColor = brush.Color;
                titleBar.InactiveBackgroundColor = brush.Color;
                titleBar.ButtonBackgroundColor = brush.Color;
                titleBar.ButtonInactiveBackgroundColor = brush.Color;
            }
        }

        private void UpdateExtendIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = ExtendIntoTitleBar;
        }

        private void OnBackgroundChanged(DependencyObject sender, DependencyProperty dp) => UpdateTitleBar();
        private void HeadersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedIndex = Items.IndexOf(e.ClickedItem);
        }

        private static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = obj as Ribbon;
            if (ribbon == null)
                return;

            int newIndex = (int)e.NewValue;
            ribbon.SelectedItem = newIndex < 0 ? null : ribbon.Items[newIndex];
        }

        private static void OnExtendIntoTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = d as Ribbon;
            ribbon?.UpdateExtendIntoTitleBar();
        }
    }
}
