import cv2
from picamera2 import Picamera2
from picfuncs import send_data, get_border_coordiantes, get_positions
import time
import socket
import json

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration(sensor={"output_size": (1640, 1232)},  main = {"format" : "RGB888"}))
picamera2.start()
time.sleep(2)

# Set up Aruco parameters
arucoDict = cv2.aruco.getPredefinedDictionary(cv2.aruco.DICT_5X5_50)
arucoParams = cv2.aruco.DetectorParameters_create()

# Init server adress
server_address = "172.22.0.6"
server_port = 6944

print("Setting up connection")
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect((server_address, server_port))
print("Connection established")


# Init position dictionary
border_coords = []
car_positions = {}

# Get border coordinates
print("Getting border coordinates...")

border_coords = get_border_coordiantes(picamera2, arucoDict, arucoParams)

while len(border_coords) != 2:
    print("Border markers not found. Trying again...")
    border_coords = get_border_coordiantes(picamera2, arucoDict, arucoParams)

print("Got border coordinates:")
print(border_coords)

# Calculate the border coordinates for roi
top_left_border = (border_coords[0][0], border_coords[1][1])
bottom_right_border = (border_coords[1][0], border_coords[0][1])

# Running tracking
print("Running tracking, press q to stop...")
while True:
    frame = picamera2.capture_array()
    #roi = frame
    roi = frame[top_left_border[1]:bottom_right_border[1], top_left_border[0]:bottom_right_border[0]]
    cv2.imshow("Frame: " ,roi)

    (corners, ids, rejected) = cv2.aruco.detectMarkers(roi, arucoDict, parameters=arucoParams)


    if len(corners) != 0:
        # Clear
        car_positions.clear() 
        # Get positions
        car_positions = get_positions(corners, ids,border_coords)

    #print(car_positions)
            
    # Send data
    client_socket.sendall(json.dumps(car_positions).encode("utf-8"))
    
    # Display the frame
    #cv2.imshow("Frame", frame)

    # Check for the 'q' key
    if cv2.waitKey(1) & 0xFF == ord("q"):
        print("Stopping program...")
        break
    
picamera2.stop()
cv2.destroyAllWindows()
print("Program stopped.")