# Functions file

# Imports
import cv2
import socket
import json
import numpy as np
from collections import deque
from scipy.fft import fft
from picamera2 import Picamera2


# Send data function
def send_data(data, host, port):
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
        s.connect((host, port))
        s.sendall(json.dumps(data).encode("utf-8"))

# Aruco functions
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

