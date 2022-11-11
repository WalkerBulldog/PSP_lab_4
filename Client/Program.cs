using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(new IPEndPoint(IPAddress.Loopback, 8000));

            var matrix_1 = new float[,]
            {
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
                { 1, 2, 3, 4 },
            };

            var matrix_2 = new float[,]
            {
                { 1, 2, 3, 4, 5 },
                { 1, 2, 3, 4, 5 },
                { 1, 2, 3, 4, 5 },
                { 1, 2, 3, 4, 5 },
            };

            socket.Send(BitConverter.GetBytes(3));
            socket.Send(BitConverter.GetBytes(4));
            socket.Send(BitConverter.GetBytes(4));
            socket.Send(BitConverter.GetBytes(5));

            foreach (var element in matrix_1)
            {
                socket.Send(BitConverter.GetBytes(element));
            }

            foreach (var element in matrix_2)
            {
                socket.Send(BitConverter.GetBytes(element));
            }

            var resultMatrix = new float[3, 5];

            for (var rowIndex = 0; rowIndex < 3; ++rowIndex)
            {
                for (var columnIndex = 0; columnIndex < 5; ++columnIndex)
                {
                    var buffer = new byte[4];

                    socket.Receive(buffer);

                    resultMatrix[rowIndex, columnIndex] = BitConverter.ToSingle(buffer);

                    Console.Write("{0, -5}", resultMatrix[rowIndex, columnIndex]);
                }

                Console.WriteLine();
            }
        }
    }
}