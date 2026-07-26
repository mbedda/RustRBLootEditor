using System.Windows;

namespace RustRBLootEditor
{
    public partial class ScrollableMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public ScrollableMessageBox()
        {
            InitializeComponent();
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            var msgBox = new ScrollableMessageBox();

            if (owner != null)
            {
                msgBox.Owner = owner;
                msgBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                msgBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            msgBox.Title = caption;
            msgBox.TitleTextBlock.Text = caption;
            msgBox.MessageTextBox.Text = messageBoxText;

            switch (button)
            {
                case MessageBoxButton.OK:
                    msgBox.BtnOk.Visibility = Visibility.Visible;
                    msgBox.BtnOk.IsDefault = true;
                    break;
                case MessageBoxButton.OKCancel:
                    msgBox.BtnOk.Visibility = Visibility.Visible;
                    msgBox.BtnCancel.Visibility = Visibility.Visible;
                    msgBox.BtnOk.IsDefault = true;
                    msgBox.BtnCancel.IsCancel = true;
                    break;
                case MessageBoxButton.YesNo:
                    msgBox.BtnYes.Visibility = Visibility.Visible;
                    msgBox.BtnNo.Visibility = Visibility.Visible;
                    msgBox.BtnYes.IsDefault = true;
                    msgBox.BtnNo.IsCancel = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    msgBox.BtnYes.Visibility = Visibility.Visible;
                    msgBox.BtnNo.Visibility = Visibility.Visible;
                    msgBox.BtnCancel.Visibility = Visibility.Visible;
                    msgBox.BtnYes.IsDefault = true;
                    msgBox.BtnCancel.IsCancel = true;
                    break;
            }

            msgBox.ShowDialog();
            return msgBox.Result;
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            return Show(Application.Current?.MainWindow!, messageBoxText, caption, button, icon);
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            DialogResult = true;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            DialogResult = false;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }
    }
}
