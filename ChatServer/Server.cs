using System.Net;
using System.Net.Sockets;

public class Server
{
    private TcpListener tcpListener;
    private List<TcpClient> clients;
    private readonly int port = 8888;

    public Server()
    {
        clients = new List<TcpClient>();
    }

    public async Task StartServer()
    {
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        Console.WriteLine($"Server started on port {port}");

        while (true)
        {
            TcpClient client = await tcpListener.AcceptTcpClientAsync();
            clients.Add(client);
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Gửi tin nhắn đến tất cả clients khác
                await BroadcastMessage(message, client);
            }
            catch
            {
                break;
            }
        }

        clients.Remove(client);
        client.Close();
    }

    private async Task BroadcastMessage(string message, TcpClient sender)
    {
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                }
                catch
                {
                    // Xử lý khi client đã disconnect
                    clients.Remove(client);
                }
            }
        }
    }
}