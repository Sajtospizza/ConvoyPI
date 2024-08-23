using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the server address and port
            string ip = "172.22.0.192";
            int port = 6944;

            // Create a TCP/IP socket
            TcpListener server = new TcpListener(IPAddress.Parse(ip), port);

            // Start listening for incoming connections
            server.Start();
            Console.WriteLine("Server started, waiting for connections...");

            // Accept a client connection
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            // Get a stream object for reading
            NetworkStream stream = client.GetStream();

            // Buffer for reading data
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                // Read the data sent by the client
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Output received JSON string
                Console.WriteLine("Received JSON: " + jsonString);
            }
        }
    }
}

