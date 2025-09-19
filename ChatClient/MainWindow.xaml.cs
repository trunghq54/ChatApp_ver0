using System.Windows;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private ChatClient1 chatClient;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChat();
        }

        private async void InitializeChat()
        {
            chatClient = new ChatClient1();
            chatClient.MessageReceived += ChatClient_MessageReceived;

            try
            {
                await chatClient.ConnectToServer();
                _ = chatClient.StartReceiving();
            }
            catch
            {
                MessageBox.Show("Không thể kết nối đến server!");
            }

            sendButton.Click += SendButton_Click;
            messageTextBox.KeyDown += MessageTextBox_KeyDown;
            emojiButton.Click += EmojiButton_Click;
        }

        private void ChatClient_MessageReceived(object sender, string message)
        {
            Dispatcher.Invoke(() =>
            {
                messagesList.Items.Add(message);
            });
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private void MessageTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                _ = SendMessage();
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrEmpty(messageTextBox.Text)) return;

            try
            {
                await chatClient.SendMessage(messageTextBox.Text);
                messagesList.Items.Add($"Me: {messageTextBox.Text}");
                messageTextBox.Clear();
            }
            catch
            {
                MessageBox.Show("Không thể gửi tin nhắn!");
            }
        }

        private void EmojiButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Hiện popup chọn emoji
            // Tạm thời thêm emoji mặc định vào textbox
            messageTextBox.Text += "😊";
        }
    }
}