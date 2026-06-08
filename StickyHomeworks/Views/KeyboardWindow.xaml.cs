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
    private bool _isLoaded;

    public KeyboardWindow()
    {
        InitializeComponent();
        KeyboardControl.CloseRequested += (s, e) => Hide();
        KeyboardControl.DoneRequested += (s, e) => Hide();
        KeyboardControl.DragHandlePressed += (s, e) =>
        {
            if (e is MouseButtonEventArgs mbe)
            {
                _isDragging = true;
                _dragStartPoint = mbe.GetPosition(this);
                CaptureMouse();
            }
        };
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
        PositionAtBottom();
        PlaySlideUpAnimation();
    }

    private void PlaySlideUpAnimation()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;
        var height = ActualHeight > 0 ? ActualHeight : 400;
        var targetTop = Math.Clamp(screen.Bottom / dpiY - height, screen.Top / dpiY, screen.Bottom / dpiY - height);

        // 设置动画的起始值和终止值
        var storyboard = (Storyboard)FindResource("SlideUpStoryboard");
        var animation = (DoubleAnimation)storyboard.Children[0];
        animation.From = Top;
        animation.To = targetTop;
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

        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var screenWidth = screen.Width / dpiX;

        // 键盘宽度占屏幕 95%，最小 600，最大 1200
        Width = Math.Clamp(screenWidth * 0.95, 600, 1200);

        // 水平居中
        Left = Math.Clamp((screenWidth - Width) / 2, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        // 初始位置在屏幕底部（动画开始位置）
        Top = screen.Bottom / (source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0);

        Show();
        Activate();

        // 如果已经加载过（非首次显示），手动定位和播放动画
        if (_isLoaded)
        {
            PositionAtBottom();
            PlaySlideUpAnimation();
        }
    }

    public void PositionAtBottom()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var screenWidth = screen.Width / dpiX;
        Width = Math.Clamp(screenWidth * 0.95, 600, 1200);

        var height = ActualHeight > 0 ? ActualHeight : 400;
        Left = Math.Clamp((screenWidth - Width) / 2, screen.Left / dpiX, (screen.Right / dpiX) - Width);
        Top = Math.Clamp(screen.Bottom / dpiY - height, screen.Top / dpiY, screen.Bottom / dpiY - height);
    }

    public void EnsureInScreen()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        var source = PresentationSource.FromVisual(this);
        var dpiX = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;
        var dpiY = source?.CompositionTarget?.TransformToDevice.M22 ?? 1.0;

        var height = ActualHeight > 0 ? ActualHeight : 400;
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

    protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
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
