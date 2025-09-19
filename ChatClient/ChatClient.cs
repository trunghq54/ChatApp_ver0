using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatClient
{
    public class ChatClient1
    {
        private TcpClient client;
        private string serverIp = "127.0.0.1";
        private int serverPort = 8888;

        public async Task ConnectToServer()
        {
            client = new TcpClient();
            await client.ConnectAsync(serverIp, serverPort);
        }

        public async Task SendMessage(string message)
        {
            if (client == null || !client.Connected) return;

            NetworkStream stream = client.GetStream();
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
        }

        public async Task StartReceiving()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Cần thêm event để thông báo message mới cho UI
                MessageReceived?.Invoke(this, message);
            }
        }

        // Event để thông báo có tin nhắn mới
        public event EventHandler<string> MessageReceived;
    }
}