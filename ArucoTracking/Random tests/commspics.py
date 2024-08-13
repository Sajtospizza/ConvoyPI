import time
import socket
import random
import cv2
import numpy as np

def string_to_numpy_array(array_str):
    # Clean up the string to make it suitable for conversion
    cleaned_str = array_str.replace('[', '').replace(']', '').replace('\n', ' ').replace('...', '')

    # Convert the string to a list of integers
    array_data = np.fromstring(cleaned_str, sep=' ')

    # Infer dimensions
    num_elements = len(array_data)
    
    # Determine the shape by identifying patterns in the original string
    rows = array_str.count('[') // 3  # Assuming each row starts with '[' and we have 3 dimensions
    cols = array_str.count('[') // rows
    channels = 4  # Assuming each sub-array has 4 channels as per the example
    
    # Reshape the flat array into the desired shape
    result_array = array_data.reshape((rows, cols, channels))

    return result_array

buffersize = 1024
ServerAddress = ("172.22.0.188",6969)
SSocket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
initmessage = "Initialize..."
initmessage = initmessage.encode("utf-8")
SSocket.sendto(initmessage, ServerAddress)
print("Message sent!")
message = "Doesnt work"
while True:
    message, address = SSocket.recvfrom(buffersize)
    message = message.decode("utf-8")
    print(message)
    #frame = string_to_numpy_array(message)
    #print(frame)
    #cv2.imshow("frame: ", message)