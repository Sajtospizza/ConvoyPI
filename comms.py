import socket
import json

buffersize = 1024
ServerAddress = ("172.22.0.188", 6944)
SSocket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
SSocket.bind(ServerAddress)

print("Server up and waiting...")
while True:
    data, address = SSocket.recvfrom(buffersize)
    data = json.loads(data.decode("utf-8"))
    print(f"Received: {data}")
