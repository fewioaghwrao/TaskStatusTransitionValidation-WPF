using System.Windows;
using System.Windows.Media.Animation;

namespace TaskStatusWpf.Views;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog(
        string title,
        string message,
        string okText = "OK",
        string cancelText = "キャンセル")
    {
        InitializeComponent();

        TitleText.Text = title;
        MessageText.Text = message;

        OkButton.Content = okText;
        CancelButton.Content = cancelText;
    }

    public bool Result { get; private set; }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (FindResource("FadeInStoryboard") is Storyboard sb)
            sb.Begin(this);
        else
            Opacity = 1;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        DialogResult = false;
    }

    private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ReferenceEquals(e.OriginalSource, sender))
        {
            Result = false;
            DialogResult = false;
        }
    }

    // ✅ 呼び出しを簡単にするヘルパー（任意）
    public static bool Show(Window owner, string title, string message, string okText, string cancelText)
    {
        var dlg = new ConfirmDialog(title, message, okText, cancelText)
        {
            Owner = owner
        };
        return dlg.ShowDialog() == true;
    }
}