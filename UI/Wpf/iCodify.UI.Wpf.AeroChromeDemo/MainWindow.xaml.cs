using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace iCodify.UI.Wpf.AeroChromeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "AeroChrome Demo";

            // Försök aktivera “Aero”/Mica/Acrylic via DWM om tillgängligt, annars fallback-borste från XAML.
            AeroBackdrop.TryApply(this);

            // Håll Max/Restore-symbol i synk
            StateChanged += (_, __) => UpdateMaxRestoreGlyph();
            Loaded += (_, __) => UpdateMaxRestoreGlyph();
            SourceInitialized += (_, __) => AeroBackdrop.ApplyRoundedCorners(this); // små rundade hörn om OS stödjer
        }

        private void UpdateMaxRestoreGlyph()
        {
            if (MaxRestoreButton == null) return;
            MaxRestoreButton.Content = WindowState == WindowState.Maximized ? "❐" : "▢";
        }

        // --- Titelradinteraktioner ---
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) // dubbelklick = maximera/återställ
            {
                ToggleMaximizeRestore();
            }
            else
            {
                DragMove();
            }
        }

        private void Icon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Close();
            }
            else if (e.ButtonState == MouseButtonState.Pressed)
            {
                ShowSystemMenuAtMouse();
            }
        }

        private void TitleBar_RightClickForSystemMenu(object sender, MouseButtonEventArgs e)
        {
            ShowSystemMenuAtMouse();
        }

        private void ShowSystemMenuAtMouse()
        {
            var p = PointToScreen(Mouse.GetPosition(this));
            SystemCommands.ShowSystemMenu(this, p);
        }

        // --- Knappar ---
        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void MaxRestore_Click(object sender, RoutedEventArgs e)
            => ToggleMaximizeRestore();

        private void ToggleMaximizeRestore()
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        // Säkerställ att ”maximerat” respekterar arbetsytans kanter på flera skärmar
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroBackdrop.FixMaximizedBounds(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            AeroBackdrop.Cleanup(this);
        }
    }
}