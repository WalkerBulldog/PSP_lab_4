using System.Net;
using System.Net.Sockets;

namespace lab4
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Configuring server with AddressFamily.InterNetwork, SocketType.Stream and ProtocolType.Tcp...");
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Done!");
            serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 8000));
            Console.WriteLine($"Listening port 8000...");
            serverSocket.Listen();
            int i = 1;
            while (true)
            {
                var clientSocket = serverSocket.Accept();
                Console.WriteLine($"Accepted client #{i++}!");
                ThreadPool.QueueUserWorkItem(ClientCallback, clientSocket);
            }
        }

        private static void ClientCallback(object? state)
        {
            var clientSocket = (Socket)state!;

            var rowCount_1 = clientSocket.ReceiveInt32();
            var columnCount_1 = clientSocket.ReceiveInt32();
            var rowCount_2 = clientSocket.ReceiveInt32();
            var columnCount_2 = clientSocket.ReceiveInt32();

            var matrix_1 = new float[rowCount_1, columnCount_1];
            var matrix_2 = new float[rowCount_2, columnCount_2];

            for (var rowIndex = 0; rowIndex < rowCount_1; ++rowIndex)
            {
                for (var columnIndex = 0; columnIndex < columnCount_1; ++columnIndex)
                {
                    matrix_1[rowIndex, columnIndex] = clientSocket.ReceiveSingle();
                }
            }

            for (var rowIndex = 0; rowIndex < rowCount_2; ++rowIndex)
            {
                for (var columnIndex = 0; columnIndex < columnCount_2; ++columnIndex)
                {
                    matrix_2[rowIndex, columnIndex] = clientSocket.ReceiveSingle();
                }
            }

            var resultMatrix = new float[rowCount_1, columnCount_2];

            var n = rowCount_2;

            for (var rowIndex = 0; rowIndex < rowCount_1; ++rowIndex)
            {
                for (var columnIndex = 0; columnIndex < columnCount_2; ++columnIndex)
                {
                    var value = default(float);

                    for (var i = 0; i < n; ++i)
                    {
                        value += matrix_1[rowIndex, i] * matrix_2[i, columnIndex];
                    }

                    resultMatrix[rowIndex, columnIndex] = value;
                }
            }

            for (var rowIndex = 0; rowIndex < rowCount_1; ++rowIndex)
            {
                for (var columnIndex = 0; columnIndex < columnCount_2; ++columnIndex)
                {
                    clientSocket.Send(BitConverter.GetBytes(resultMatrix[rowIndex, columnIndex]));
                }
            }
            clientSocket.Close();
        }
    }

    internal static class SocketExtensions
    {
        public static int ReceiveInt32(this Socket socket)
        {
            var buffer = new byte[4];

            socket.Receive(buffer);

            return BitConverter.ToInt32(buffer);
        }

        public static float ReceiveSingle(this Socket socket)
        {
            var buffer = new byte[4];

            socket.Receive(buffer);

            return BitConverter.ToSingle(buffer);
        }
    }
}