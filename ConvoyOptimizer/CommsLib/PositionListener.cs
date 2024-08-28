using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OptimizerFrontend.CommsLib
{
    internal class PositionListener
    {
        // Define the server address and port
        public string Ip;
        public int Port;
        public Dictionary<string, List<double>> outcoordinates;
        public Dictionary<string, List<double>> coordinates;
        

        // TcpListener to listen for incoming connections
        TcpListener server;
        TcpClient client;

        // Constuctor
        public PositionListener(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
            coordinates = new Dictionary<string, List<double>>();
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

        // Function to receive positions
        public void ReceiveData()
        {
            // Get a stream object for reading
            NetworkStream stream = client.GetStream();

            // Buffer for reading data
            byte[] buffer = new byte[1024];
            int bytesRead;
            Dictionary<string, List<double>> coordinates;

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

                // Fuck this
                try
                {
                    coordinates = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonString);
                    //Debug.WriteLine(coordinates);
                }
                
                catch (Exception e)
                {
                    //Debug.WriteLine(e);
          
                    continue;
                }
               


                outcoordinates = coordinates;
            }
        }

        // Function to get the cars information
        public Dictionary<string, List<double>> ReceiveCars()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            Dictionary<string, List<double>> coordinates;

            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            coordinates = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonString);
            
            return coordinates;
        }
    }
}
