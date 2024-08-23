import cv2
from picamera2 import Picamera2
from picfuncs import send_data, get_border_coordiantes, get_positions
import time

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
server_address = "172.22.0.54"
server_port = 6944

# Init position dictionary
border_coords = []
led_positions = {}

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
    roi = frame[top_left_border[1]:bottom_right_border[1], top_left_border[0]:bottom_right_border[0]]

    (corners, ids, rejected) = cv2.aruco.detectMarkers(roi, arucoDict, parameters=arucoParams)

    if len(corners) != 0:
        # Clear
        led_positions.clear() 
        # Get positions
        led_positions = get_positions(corners, ids)
            
    # Send data
    send_data(led_positions, server_address, server_port)
    
    # Display the frame
    cv2.imshow("Frame", frame)

    # Check for the 'q' key
    if cv2.waitKey(1) & 0xFF == ord("q"):
        print("Stopping program...")
        break
    
picamera2.stop()
cv2.destroyAllWindows()
print("Program stopped.")