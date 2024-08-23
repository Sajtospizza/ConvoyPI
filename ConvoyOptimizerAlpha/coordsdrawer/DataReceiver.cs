using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Diagnostics;

namespace coordsdrawer
{
    internal class DataReceiver
    {
        // Define the server address and port
        public string Ip;
        public int Port;
        public Dictionary<string, List<double>> outcoordinates;

        TcpListener server;
        TcpClient client;

        public DataReceiver(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }

        public void StartListening()
        {
            // Create a TCP/IP socket
            server = new TcpListener(IPAddress.Parse(Ip), Port);

            // Start listening for incoming connections
            server.Start();
            Debug.WriteLine("Server started, listening");
            client = server.AcceptTcpClient();
            Debug.WriteLine("Client connected");
        }

        public void ReceiveData()
        {
            // Get a stream object for reading
            NetworkStream stream = client.GetStream();

            // Buffer for reading data
            byte[] buffer = new byte[1024];
            int bytesRead;
            Dictionary<string, List<double>>  coordinates = null;
            Dictionary<string, List<double>> prevcoordinates = null;

            while (true)
            {
                // Read the data sent by the client
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                int count = jsonString.Count(c => c == '}');
                if (count > 1)
                {
                    jsonString = jsonString.Substring(0, jsonString.IndexOf('}') + 1);
                }
  
                coordinates = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonString);

                Debug.WriteLine($"Key: {coordinates.Keys.First()}, Value: [{string.Join(", ", coordinates.Values.First())}]");

                outcoordinates = coordinates;
            }

        }


          

            
}
}
