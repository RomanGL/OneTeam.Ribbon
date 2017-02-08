using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
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
        private CoreApplicationViewTitleBar coreTitleBar;
        private TitleBar titleBar;
        private TextBlock title;
        private Grid placeholder;
        private QuickAccessToolbarButton backButton;
        private FrameworkElement tabContentPresenter;
        private Visibility backButtonVisibility;
        private bool isWindowDeactivated;
        private ContentPresenter quickAccessToolbar;
        private Button menuButton;

        public Ribbon()
        {
            DefaultStyleKey = typeof(Ribbon);
        }

        public event RoutedEventHandler FileClick;

        private void OnFileClick()
        {
            RoutedEventHandler eh = FileClick;
            eh?.Invoke(this, new RoutedEventArgs());
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

        public bool IsTitleBarTabletModeVisible
        {
            get { return (bool)GetValue(IsTitleBarTabletModeVisibleProperty); }
            set { SetValue(IsTitleBarTabletModeVisibleProperty, value); }
        }

        public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        public QuickAccessToolbar QuickAccessToolbar
        {
            get { return (QuickAccessToolbar)GetValue(QuickAccessToolbarProperty); }
            set { SetValue(QuickAccessToolbarProperty, value); }
        }

        public bool IsMenuEnabled
        {
            get { return (bool)GetValue(IsMenuEnabledProperty); }
            set { SetValue(IsMenuEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsMenuEnabledProperty =
            DependencyProperty.Register(nameof(IsMenuEnabled), typeof(bool),
                typeof(Ribbon), new PropertyMetadata(true, OnIsMenuButonEnabledChanged));

        public static readonly DependencyProperty ExtendIntoTitleBarProperty =
            DependencyProperty.Register(nameof(ExtendIntoTitleBar), typeof(bool), 
                typeof(Ribbon), new PropertyMetadata(true, OnExtendIntoTitleBarChanged));

        public static readonly DependencyProperty IsTitleBarTabletModeVisibleProperty =
            DependencyProperty.Register(nameof(IsTitleBarTabletModeVisible), typeof(bool),
                typeof(Ribbon), new PropertyMetadata(true, null));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), 
                typeof(Ribbon), new PropertyMetadata(-1, OnSelectedIndexChanged));
        
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), 
                typeof(Ribbon), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), 
                typeof(Ribbon), new PropertyMetadata(null));

        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register(nameof(BackButtonVisibility), typeof(Visibility),
                typeof(Ribbon), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty QuickAccessToolbarProperty =
            DependencyProperty.Register(nameof(QuickAccessToolbar), typeof(QuickAccessToolbar),
                typeof(Ribbon), new PropertyMetadata(null));

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Height = Window.Current.Bounds.Height;
            return base.MeasureOverride(availableSize);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            backButtonVisibility = BackButtonVisibility;

            backgroundElement = GetTemplateChild(nameof(backgroundElement)) as Rectangle;
            headersListView = GetTemplateChild(nameof(headersListView)) as ListView;
            titleBar = GetTemplateChild(nameof(titleBar)) as TitleBar;
            title = GetTemplateChild(nameof(title)) as TextBlock;
            placeholder = GetTemplateChild(nameof(placeholder)) as Grid;
            backButton = GetTemplateChild(nameof(backButton)) as QuickAccessToolbarButton;
            tabContentPresenter = GetTemplateChild(nameof(tabContentPresenter)) as FrameworkElement;
            quickAccessToolbar = GetTemplateChild(nameof(quickAccessToolbar)) as ContentPresenter;
            menuButton = GetTemplateChild(nameof(menuButton)) as Button;

            menuButton.Click += MenuButton_Click;

            headersListView.ItemsSource = Items;

            if (Items?.Count > 0)
                SelectedIndex = 0;

            InvalidateMeasure();

            if (!DesignMode.DesignModeEnabled)
            {
                Window.Current.SetTitleBar(backgroundElement);

                UpdateTitleBarBackground();
                UpdateTitleBarForeground();
                UpdateExtendIntoTitleBar();

                coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

                Window.Current.Activated += Window_Activated;
            }

            UpdateMenuButton();
            UpdateSelectedIndex();

            RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundPropertyChanged);
            RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundPropertyChanged);

            Background?.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBackgroundPropertyChanged);
            Foreground?.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnForegroundPropertyChanged);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            OnFileClick();
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            BackButtonVisibility = sender.IsVisible ? backButtonVisibility : Visibility.Collapsed;

            if (IsTitleBarTabletModeVisible)
                return;

            if (sender.IsVisible)
            {
                titleBar.Visibility = Visibility.Visible;
                placeholder.Height = 80;
                backgroundElement.Height = 80;
            }
            else
            {
                titleBar.Visibility = Visibility.Collapsed;
                placeholder.Height = 44;
                backgroundElement.Height = 44;
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            isWindowDeactivated = e.WindowActivationState == CoreWindowActivationState.Deactivated;

            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                title.Opacity = 1;
                backButton.Opacity = 1;
                quickAccessToolbar.Opacity = 1;
            }
            else
            {
                title.Opacity = 0.5;
                backButton.Opacity = 0.5;
                quickAccessToolbar.Opacity = 0.5;
            }

            UpdateTitleBarForeground();
        }

        private void UpdateMenuButton()
        {
            if (menuButton == null)
                return;

            menuButton.Visibility = IsMenuEnabled ? Visibility.Visible : Visibility.Collapsed;
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
                titleBar.ButtonForegroundColor = Colors.Red;
            }
            else
            {
                titleBar.ForegroundColor = brush.Color;
                titleBar.ButtonForegroundColor = brush.Color;

                if (titleBar.BackgroundColor.HasValue)
                {
                    Color color = titleBar.BackgroundColor.Value;
                    titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255,
                        (byte)((brush.Color.R + color.R) / 2),
                        (byte)((brush.Color.G + color.G) / 2),
                        (byte)((brush.Color.B + color.B) / 2));
                }
            }
        }

        private void UpdateTabsForeground(int selectedIndex)
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

            for (int i = 0; i < Items.Count; i++)
            {
                var control = Items[i] as Control;
                if (control == null)
                    continue;

                control.Foreground = brush;

                if (selectedIndex == i)
                    control.Foreground = Background;
            }
        }

        private void UpdateSelectedIndex()
        {
            SelectedItem = SelectedIndex < 0 ? null : Items?[SelectedIndex];
            UpdateTabsForeground(SelectedIndex);
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

            UpdateTabsForeground(0);
        }

        private static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = obj as Ribbon;
            ribbon?.UpdateSelectedIndex();
        }

        private static void OnExtendIntoTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = d as Ribbon;
            ribbon?.UpdateExtendIntoTitleBar();
        }

        private static void OnIsMenuButonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = d as Ribbon;
            ribbon?.UpdateMenuButton();
        }
    }
}
