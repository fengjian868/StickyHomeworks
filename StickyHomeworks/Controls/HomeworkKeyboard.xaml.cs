using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using StickyHomeworks.Models;

namespace StickyHomeworks.Controls;

public partial class HomeworkKeyboard : UserControl
{
    public static readonly DependencyProperty TargetRichTextBoxProperty = DependencyProperty.Register(
        nameof(TargetRichTextBox), typeof(RichTextBox), typeof(HomeworkKeyboard),
        new PropertyMetadata(null));

    public RichTextBox? TargetRichTextBox
    {
        get => (RichTextBox?)GetValue(TargetRichTextBoxProperty);
        set => SetValue(TargetRichTextBoxProperty, value);
    }

    public static readonly DependencyProperty CustomButtonsProperty = DependencyProperty.Register(
        nameof(CustomButtons), typeof(ObservableCollection<KeyboardButton>), typeof(HomeworkKeyboard),
        new PropertyMetadata(new ObservableCollection<KeyboardButton>(), OnCustomButtonsChanged));

    public ObservableCollection<KeyboardButton> CustomButtons
    {
        get => (ObservableCollection<KeyboardButton>)GetValue(CustomButtonsProperty);
        set => SetValue(CustomButtonsProperty, value);
    }

    public event EventHandler? CloseRequested;

    public HomeworkKeyboard()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitializeDefaultButtons();
        RefreshCustomButtons();
    }

    private static void OnCustomButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HomeworkKeyboard keyboard)
        {
            keyboard.RefreshCustomButtons();
        }
    }

    private void InitializeDefaultButtons()
    {
        // 数字
        AddButtons(NumberPanel, new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }, "number");

        // 中文数字
        AddButtons(ChineseNumberPanel, new[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" }, "chineseNumber");

        // 书本
        AddButtons(BookPanel, new[] { "必修", "选修", "大本", "小本", "作业本", "书本", "错题本", "作文本", "周记本" }, "book");

        // 时间1
        AddButtons(TimePanel1, new[] { "早上", "下午", "晚上", "今天", "昨天", "明天", "后天", "月", "日", "星期" }, "time");

        // 时间2
        AddButtons(TimePanel2, new[] { "第", "节", "上课", "下课", "回来", "点", "课", "前", "后", "时" }, "time");

        // 内容
        AddButtons(ContentPanel, new[] { "检查", "订正", "修改", "补", "收", "发的", "写在", "至", "题", "课时" }, "content");

        // 符号
        AddButtons(SymbolPanel, new[] { "、", "—", "；", "：", "\"", "\"", "《", "》", "（", "）" }, "symbol");

        // 常用字母
        AddButtons(LetterPanel, new[] { "A", "B", "C", "D", "P" }, "letter");
    }

    private void AddButtons(Panel panel, string[] texts, string category)
    {
        panel.Children.Clear();
        foreach (var text in texts)
        {
            var button = new Button
            {
                Content = text,
                Style = (Style)FindResource("Win11KeyStyle"),
                Tag = new KeyboardButton(text, category)
            };
            button.Click += ButtonInsert_OnClick;
            panel.Children.Add(button);
        }
    }

    private void RefreshCustomButtons()
    {
        CustomPanel.Children.Clear();
        foreach (var button in CustomButtons)
        {
            var btn = new Button
            {
                Content = button.DisplayText,
                Style = (Style)FindResource("Win11KeyStyle"),
                Tag = button
            };
            btn.Click += ButtonInsert_OnClick;
            CustomPanel.Children.Add(btn);
        }
    }

    private void ButtonInsert_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: KeyboardButton button }) return;
        InsertText(button.Text);
    }

    private void InsertText(string text)
    {
        if (TargetRichTextBox == null) return;

        TargetRichTextBox.Focus();
        // 始终将光标移到文档末尾再插入
        TargetRichTextBox.CaretPosition = TargetRichTextBox.Document.ContentEnd;
        var selection = TargetRichTextBox.Selection;
        selection.Text = text;
        // 插入后光标保持在末尾
        TargetRichTextBox.CaretPosition = TargetRichTextBox.Document.ContentEnd;
    }

    private void ButtonBackspace_OnClick(object sender, RoutedEventArgs e)
    {
        if (TargetRichTextBox == null) return;

        var selection = TargetRichTextBox.Selection;
        if (!selection.IsEmpty)
        {
            selection.Text = "";
        }
        else
        {
            var caret = TargetRichTextBox.CaretPosition;
            var prev = caret.GetPositionAtOffset(-1, System.Windows.Documents.LogicalDirection.Backward);
            if (prev != null)
            {
                selection.Select(prev, caret);
                selection.Text = "";
            }
        }

        TargetRichTextBox.Focus();
    }

    private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
    {
        if (TargetRichTextBox == null) return;

        var selection = TargetRichTextBox.Selection;
        selection.Text = "\r\n";
        TargetRichTextBox.Focus();
    }

    private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
