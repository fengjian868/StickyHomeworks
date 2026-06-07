using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using StickyHomeworks.Controls;

namespace StickyHomeworks.Views;

public partial class KeyboardWindow : Window
{
    public HomeworkKeyboard Keyboard => KeyboardControl;
    private bool _isDragging;
    private Point _dragStartPoint;

    public KeyboardWindow()
    {
        InitializeComponent();
        KeyboardControl.CloseRequested += (s, e) => Hide();
        KeyboardControl.MouseLeftButtonDown += OnMouseLeftButtonDown;
        KeyboardControl.MouseMove += OnMouseMove;
        KeyboardControl.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    public void ShowKeyboard()
    {
        if (IsVisible) return;
        PositionAtBottom();
        Show();
        var storyboard = (Storyboard)FindResource("SlideUpStoryboard");
        storyboard.Begin(this);
    }

    public void PositionAtBottom()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.7, 350, 650);

        Left = (screenWidth - Width) / 2;
        Top = screen.Bottom / dpiY - ActualHeight;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) return;
        _isDragging = true;
        _dragStartPoint = e.GetPosition(this);
        KeyboardControl.CaptureMouse();
    }

    private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isDragging) return;
        var current = e.GetPosition(this);
        Left += current.X - _dragStartPoint.X;
        Top += current.Y - _dragStartPoint.Y;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        KeyboardControl.ReleaseMouseCapture();
    }
}
