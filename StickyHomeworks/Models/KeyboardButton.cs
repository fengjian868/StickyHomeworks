using CommunityToolkit.Mvvm.ComponentModel;

namespace StickyHomeworks.Models;

public class KeyboardButton : ObservableRecipient
{
    private string _text = "";
    private string _displayText = "";
    private string _category = "";

    public string Text
    {
        get => _text;
        set
        {
            if (value == _text) return;
            _text = value;
            OnPropertyChanged();
        }
    }

    public string DisplayText
    {
        get => _displayText;
        set
        {
            if (value == _displayText) return;
            _displayText = value;
            OnPropertyChanged();
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            if (value == _category) return;
            _category = value;
            OnPropertyChanged();
        }
    }

    public KeyboardButton(string text, string displayText, string category)
    {
        _text = text;
        _displayText = displayText;
        _category = category;
    }

    public KeyboardButton(string text, string category) : this(text, text, category)
    {
    }

    public KeyboardButton()
    {
    }
}
