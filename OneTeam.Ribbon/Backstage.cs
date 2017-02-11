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
    public sealed class Backstage : ItemsControl
    {
        private Rectangle backgroundElement;
        private CoreApplicationViewTitleBar coreTitleBar;
        private TitleBar titleBar;
        private TextBlock title;
        private Grid placeholder;
        private QuickAccessToolbarButton backButton;
        private bool isWindowDeactivated;

        public Backstage()
        {
            DefaultStyleKey = typeof(Backstage);
        }

        public event RoutedEventHandler BackButtonClick;

        private void OnBackButtonClick()
        {
            RoutedEventHandler eh = BackButtonClick;
            eh?.Invoke(this, new RoutedEventArgs());
        }

        public bool ExtendIntoTitleBar
        {
            get { return (bool)GetValue(ExtendIntoTitleBarProperty); }
            set { SetValue(ExtendIntoTitleBarProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public bool IsTitleBarTabletModeVisible
        {
            get { return (bool)GetValue(IsTitleBarTabletModeVisibleProperty); }
            set { SetValue(IsTitleBarTabletModeVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsTitleBarTabletModeVisibleProperty =
            DependencyProperty.Register(nameof(IsTitleBarTabletModeVisible), typeof(bool),
                typeof(Backstage), new PropertyMetadata(true, null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string),
                typeof(Backstage), new PropertyMetadata(null));

        public static readonly DependencyProperty ExtendIntoTitleBarProperty =
            DependencyProperty.Register(nameof(ExtendIntoTitleBar), typeof(bool),
                typeof(Backstage), new PropertyMetadata(true, OnExtendIntoTitleBarChanged));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            backgroundElement = GetTemplateChild(nameof(backgroundElement)) as Rectangle;
            titleBar = GetTemplateChild(nameof(titleBar)) as TitleBar;
            title = GetTemplateChild(nameof(title)) as TextBlock;
            placeholder = GetTemplateChild(nameof(placeholder)) as Grid;
            backButton = GetTemplateChild(nameof(backButton)) as QuickAccessToolbarButton;
            backButton.Click += BackButton_Click;

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

            RegisterPropertyChangedCallback(BackgroundProperty, OnBackgroundPropertyChanged);
            RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundPropertyChanged);

            Background?.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBackgroundPropertyChanged);
            Foreground?.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnForegroundPropertyChanged);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackButtonClick();
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (IsTitleBarTabletModeVisible)
                return;

            if (sender.IsVisible)
            {
                titleBar.Visibility = Visibility.Visible;
                placeholder.Height = 32;
                backgroundElement.Height = 32;
            }
            else
            {
                titleBar.Visibility = Visibility.Collapsed;
                placeholder.Height = 0;
                backgroundElement.Height = 0;
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            isWindowDeactivated = e.WindowActivationState == CoreWindowActivationState.Deactivated;

            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                title.Opacity = 1;
                backButton.Opacity = 1;
            }
            else
            {
                title.Opacity = 0.5;
                backButton.Opacity = 0.5;
            }

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
        }

        private static void OnExtendIntoTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = d as Backstage;
            ribbon?.UpdateExtendIntoTitleBar();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Width = Window.Current.Bounds.Width;
            return base.MeasureOverride(availableSize);
        }
    }
}
