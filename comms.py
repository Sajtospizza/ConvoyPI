import socket
import json

def setup_client(server_address, server_port):
    SenderSocket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    SenderSocket.bind((server_address, server_port))
    return SenderSocket

def receive_data(SenderSocket, buffersize):
    data, address = SenderSocket.recvfrom(buffersize)
    data = json.loads(data.decode("utf-8"))
    return data

    
