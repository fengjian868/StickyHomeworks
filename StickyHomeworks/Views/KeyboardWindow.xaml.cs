using System.Windows;
using System.Windows.Forms;
using StickyHomeworks.Controls;

namespace StickyHomeworks.Views;

public partial class KeyboardWindow : Window
{
    public HomeworkKeyboard Keyboard => KeyboardControl;

    public KeyboardWindow()
    {
        InitializeComponent();
        KeyboardControl.CloseRequested += (s, e) => Hide();
    }

    public void ShowKeyboard()
    {
        Show();
        PositionAtBottom();
    }

    public void PositionAtBottom()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.8, 400, 800);

        Left = (screenWidth - Width) / 2;
        Top = screen.Bottom / dpiY - ActualHeight;
    }
}
