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
        KeyboardControl.MouseLeftButtonDown += OnMouseLeftButtonDown;
        KeyboardControl.MouseMove += OnMouseMove;
        KeyboardControl.MouseLeftButtonUp += OnMouseLeftButtonUp;
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
            // 已可见时只更新位置
            PositionAtBottom();
            return;
        }

        // 先设置宽度，让 SizeToContent 计算高度
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.85, 400, 800);
        Left = (screenWidth - Width) / 2;
        // 不设置 Top，让 Loaded 事件定位

        Show();
    }

    private double _dpiY => PresentationSource.FromVisual(this)?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

    public void PositionAtBottom()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.85, 400, 800);

        var height = ActualHeight > 0 ? ActualHeight : 320;
        Left = Math.Clamp((screenWidth - Width) / 2, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        Top = Math.Clamp(screen.Bottom / dpiY - height, screen.Top / dpiY, screen.Bottom / dpiY - height);
    }

    public void EnsureInScreen()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var height = ActualHeight > 0 ? ActualHeight : 320;
        Left = Math.Clamp(Left, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        Top = Math.Clamp(Top, screen.Top / dpiY, (screen.Bottom / dpiY) - height);
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
        EnsureInScreen();
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        KeyboardControl.ReleaseMouseCapture();
    }
}
