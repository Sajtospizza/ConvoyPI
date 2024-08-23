# Functions file

# Imports
import cv2
import socket
import json
import numpy as np
from collections import deque
from scipy.fft import fft
from picamera2 import Picamera2

# Led track functions

# Red mask function
def get_red_mask(image):
    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    # Define HSV range for detecting red colors
    lower_red1 = np.array([0, 160, 160])  # Lower bound for red in HSV
    upper_red1 = np.array([10, 255, 255])  # Upper bound for red in HSV
    lower_red2 = np.array([160, 120, 120])  # Lower bound for red in HSV
    upper_red2 = np.array([170, 220, 220])  # Upper bound for red in HSV

    mask1 = cv2.inRange(hsv, lower_red1, upper_red1)
    mask2 = cv2.inRange(hsv, lower_red2, upper_red2)
    
    mask = mask1 + mask2
    return mask

# Identify LED positions
def identify_led_positions(frames, camera_framerate, led_frequencies, frequency_tolerance=0.5):
    num_frames = len(frames)
    fft_frames = fft(frames, axis=0)
    magnitude = np.abs(fft_frames)
    dominant_frequencies = np.argmax(magnitude[1:num_frames//2], axis=0) + 1 # Skip the zero frequency
    frequencies = dominant_frequencies * (camera_framerate / num_frames)
    
    
    led_positions = {}
    for freq in led_frequencies:
        mask = (frequencies > (freq - frequency_tolerance)) & (frequencies < (freq + frequency_tolerance))
        coords = np.column_stack(np.where(mask))
        print(coords)
        led_positions[freq] = coords
    return led_positions

# Detect LEDs function
def detect_leds(picamera2, num_frames, camera_framerate, led_frequencies, frequency_tolerance):
    frame_buffer = deque(maxlen=num_frames)
    led_positions = {freq: [] for freq in led_frequencies}
    
    for _ in range(num_frames):
        image = picamera2.capture_array()
        
        red_mask = get_red_mask(image)
        red_image = cv2.bitwise_and(image, image, mask=red_mask)
        
        gray = cv2.cvtColor(red_image, cv2.COLOR_BGR2GRAY)
        
        _, thresholded = cv2.threshold(gray, 50, 255, cv2.THRESH_BINARY)
        
        # Debug thresholed image
        cv2.imshow("Thresholded Image", thresholded)
        
        frame_buffer.append(thresholded)
        
        cv2.waitKey(1)
    
    led_positions = identify_led_positions(np.array(frame_buffer), camera_framerate, led_frequencies, frequency_tolerance)
    
    middle_points = {}
    for freq, positions in led_positions.items():
        if positions.size > 0:
            avg_y = int(np.mean(positions[:, 0]))
            avg_x = int(np.mean(positions[:, 1]))
            middle_points[freq] = [(avg_y, avg_x)]
        else:
            middle_points[freq] = []
    
    return middle_points

# Track LEDs function
def track_leds(frame, previous_positions):
    mask = get_red_mask(frame)
    tracked_positions = {freq: [] for freq in previous_positions.keys()}
    
    for freq, positions in previous_positions.items():
        for y, x in positions:
            y1, y2 = max(0, y-50), min(frame.shape[0], y+50)
            x1, x2 = max(0, x-50), min(frame.shape[1], x+50)
            roi = mask[y1:y2, x1:x2]
            contours, _ = cv2.findContours(roi, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
            if contours:
                largest_contour = max(contours, key=cv2.contourArea)
                M = cv2.moments(largest_contour)
                if M["m00"] != 0:
                    cX = int(M["m10"] / M["m00"]) + x1
                    cY = int(M["m01"] / M["m00"]) + y1
                    tracked_positions[freq].append((cY, cX))
                else:
                    tracked_positions[freq].append((y, x))
            else:
                tracked_positions[freq].append((y, x))
    
    return tracked_positions

# Draw average positions
def draw_average_positions(frame, led_positions):
    for freq, positions in led_positions.items():
        if positions:
            avg_x = int(np.mean([pos[1] for pos in positions]))
            avg_y = int(np.mean([pos[0] for pos in positions]))
            cv2.circle(frame, (avg_x, avg_y), 5, (0, 255, 0), -1)

# Send data function
def send_data(data, host, port):
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
        s.connect((host, port))
        s.sendall(json.dumps(data).encode("utf-8"))

# Aruco functions
<<<<<<< HEAD:ArucoTracking/picfuncs.py
=======
        
def get_border_coordiantes(picamera2,arucoDict,arucoParams):
    firstframe = picamera2.capture_array()
    border_coords = [None, None]
    (bordercorners, borderids,reject) = cv2.aruco.detectMarkers(firstframe, arucoDict, parameters=arucoParams)

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
            
            border_coords[~int(marker_id[0])] = (center_x, center_y)

    if border_coords.__contains__(None):
        return []
    return border_coords


# Get positions

def get_positions(corners, ids):
    led_positions = {}
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

        pos = transform_point((center_x, center_y),(1000, 500))
            
        # Update the positions in the dictionary
        led_positions[int(marker_id[0])] = pos
    return led_positions 

def transform_point(orig_point, top_right):

    orig_x, orig_y = orig_point
    bl_x, bl_y = (0,0)
    tr_x, tr_y = top_right
    
    # Calculate the width and height of the rectangle in the original coordinate system
    width_orig = tr_x - bl_x
    height_orig = tr_y - bl_y
    
    # Calculate the scale factors for x and y
    scale_x = tr_x/ width_orig
    scale_y = tr_y / height_orig
    
    # Transform the original point to the new coordinate system
    new_x = (orig_x - bl_x) * scale_x
    new_y = (orig_y - bl_y) * scale_y
    
    return (new_x, new_y)
>>>>>>> bbfddbe (tracking):picfuncs.py

