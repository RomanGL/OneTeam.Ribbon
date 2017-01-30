using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
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

        private bool isWindowDeactivated;

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

                UpdateTitleBarBackground();
                UpdateTitleBarForeground();
                UpdateExtendIntoTitleBar();

                headersListView.ItemClick += HeadersListView_ItemClick;
                Window.Current.Activated += Window_Activated;
            }

            RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundPropertyChanged);
            RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundPropertyChanged);

            Background?.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBackgroundPropertyChanged);
            Foreground?.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnForegroundPropertyChanged);
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            isWindowDeactivated = e.WindowActivationState == CoreWindowActivationState.Deactivated;
            UpdateTitleBarForeground();
        }

        private void UpdateTitleBarBackground()
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

        private void UpdateTitleBarForeground()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            var brush = Foreground as SolidColorBrush;
            if (brush == null)
            {
                titleBar.ForegroundColor = null;
                titleBar.ButtonForegroundColor = null;
            }
            else
            {
                titleBar.ForegroundColor = brush.Color;
                titleBar.ButtonForegroundColor = brush.Color;
            }
        }

        private void UpdateTabsForeground()
        {
            var brush = Foreground as SolidColorBrush;
            if (brush == null)
                return;

            foreach (var item in Items)
            {
                var control = item as Control;
                if (control == null)
                    continue;

                control.Foreground = brush;
            }
        }

        private void UpdateExtendIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = ExtendIntoTitleBar;
        }

        private void OnBackgroundPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (!DesignMode.DesignModeEnabled)
                UpdateTitleBarBackground();
        }

        private void OnForegroundPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (!DesignMode.DesignModeEnabled)
                UpdateTitleBarForeground();

            UpdateTabsForeground();
        }

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
