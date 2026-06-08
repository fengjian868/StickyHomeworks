using System.Windows;
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
        KeyboardControl.DoneRequested += (s, e) => Hide();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // 窗口加载后定位到底部并播放弹入动画
        PositionAtBottom();
        var storyboard = (Storyboard)FindResource("SlideUpStoryboard");
        storyboard.Begin(this);
    }

    public void ShowKeyboard()
    {
        if (IsVisible)
        {
            PositionAtBottom();
            Activate();
            return;
        }

        // 设置窗口宽度
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.9, 480, 900);

        // 先定位到底部（在屏幕外），然后 Show，再由动画弹入
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;
        var estimatedHeight = 380; // 预估高度
        Left = Math.Clamp((screenWidth - Width) / 2, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        Top = screen.Bottom / dpiY; // 初始在屏幕底部外，动画会把它弹上来

        Show();
        Activate();
    }

    public void PositionAtBottom()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.9, 480, 900);

        var height = ActualHeight > 0 ? ActualHeight : 380;
        Left = Math.Clamp((screenWidth - Width) / 2, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        Top = Math.Clamp(screen.Bottom / dpiY - height, screen.Top / dpiY, screen.Bottom / dpiY - height);
    }

    public void EnsureInScreen()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var height = ActualHeight > 0 ? ActualHeight : 380;
        Left = Math.Clamp(Left, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        Top = Math.Clamp(Top, screen.Top / dpiY, (screen.Bottom / dpiY) - height);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (e.ClickCount == 2) return;
        _isDragging = true;
        _dragStartPoint = e.GetPosition(this);
        CaptureMouse();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!_isDragging) return;
        var current = e.GetPosition(this);
        Left += current.X - _dragStartPoint.X;
        Top += current.Y - _dragStartPoint.Y;
        EnsureInScreen();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        _isDragging = false;
        ReleaseMouseCapture();
    }
}
