from picamera2 import Picamera2
import cv2

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration(sensor={"output_size": (1640, 1232)},  main = {"format" : "RGB888"}))
picamera2.start()

arucoDict = cv2.aruco.getPredefinedDictionary(cv2.aruco.DICT_5X5_50)
arucoParams = cv2.aruco.DetectorParameters_create()

# Init position dictionary
border_coords = []
led_positions = {}

print("Getting border coordinates...")
firstframe = picamera2.capture_array()
(bordercorners, borderids, reject) = cv2.aruco.detectMarkers(firstframe, arucoDict, parameters=arucoParams)

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
print(border_coords)
top_left = (border_coords[0][0], border_coords[1][1])
bottom_right = (border_coords[1][0], border_coords[0][1])

while True:
    image = picamera2.capture_array()
    cv2.rectangle(image, top_left, bottom_right, (0, 255, 0), 2)
    #(corners, ids, rejected) = cv2.aruco.detectMarkers(image, arucoDict,
	#parameters=arucoParams)
    #frame_markers = cv2.aruco.drawDetectedMarkers(image, corners, ids)
    cv2.imshow("Image",image)
        
    #cv2.imshow("Thresholded Image", thresholded)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break