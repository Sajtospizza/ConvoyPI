import cv2
from picamera2 import Picamera2
from picfuncs import send_data
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
server_address = "172.22.0.25"
server_port = 6944

# Init position dictionary
border_coords = []
led_positions = {}

# Get border coordinates
print("Getting border coordinates...")
firstframe = picamera2.capture_array()
(bordercorners, borderids, reject) = cv2.aruco.detectMarkers(firstframe, arucoDict, parameters=arucoParams)

led_positions.clear()  # Clear previous positions

# Markers 0 and 1 are the border markers
for (marker_corner, marker_id) in zip(bordercorners, borderids):
    if (marker_id in [0,1]):
        # Extract the marker corners
        corners = marker_corner.reshape((4, 2))
        (top_left, top_right, bottom_right, bottom_left) = corners
         
        # Convert the (x,y) coordinate pairs to integers
        top_right = (int(top_right[0]), int(top_right[1]))
        bottom_right = (int(bottom_right[0]), int(bottom_right[1]))
        bottom_left = (int(bottom_left[0]), int(bottom_left[1]))
        top_left = (int(top_left[0]), int(top_left[1]))

        center_x = int((top_left[0] + bottom_right[0]) / 2.0)
        center_y = int((top_left[1] + bottom_right[1]) / 2.0)
        
        border_coords.append((center_x, center_y))

# Calculate the border coordinates
top_left_border = (border_coords[0][0], border_coords[1][1])
bottom_right_border = (border_coords[1][0], border_coords[0][1])

# Running tracking
print("Running tracking...")
while True:
    frame = picamera2.capture_array()
    roi = frame[top_left_border[1]:bottom_right_border[1], top_left_border[0]:bottom_right_border[0]]

    (corners, ids, rejected) = cv2.aruco.detectMarkers(roi, arucoDict, parameters=arucoParams)

    if len(corners) != 0:

        # Clear
        led_positions.clear() 
        
        for (marker_corner, marker_id) in zip(corners, ids):
        
            # Extract the marker corners
            corners = marker_corner.reshape((4, 2))
            (top_left, top_right, bottom_right, bottom_left) = corners
            
            # Convert the (x,y) coordinate pairs to integers
            top_right = (int(top_right[0]), int(top_right[1]))
            bottom_right = (int(bottom_right[0]), int(bottom_right[1]))
            bottom_left = (int(bottom_left[0]), int(bottom_left[1]))
            top_left = (int(top_left[0]), int(top_left[1]))
            
            # Calculate and draw the center of the ArUco marker
            center_x = int((top_left[0] + bottom_right[0]) / 2.0)
            center_y = int((top_left[1] + bottom_right[1]) / 2.0)
            
            # Adjust coordinates to be relative to the full image
            global_center_x = top_left[0] + center_x
            global_center_y = top_left[1] + center_y

            # Update the positions in the dictionary
            led_positions[int(marker_id[0])] = (global_center_x, global_center_y)

            # Draw the ArUco marker ID on the video frame
            # The ID is always located at the center
            cv2.putText(roi, str(marker_id), 
            (center_x, center_y - 15),
            cv2.FONT_HERSHEY_SIMPLEX,
            0.5, (0, 255, 0), 2)

    # Send data
    send_data(led_positions, server_address, server_port)
    
    # Debug imshow
    #cv2.imshow("Image",roi)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

picamera2.stop()
cv2.destroyAllWindows()