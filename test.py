
import cv2
from picamera2 import Picamera2
import numpy as np


def get_red_mask(image):
    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    # ONLY PHONE
    lower_red1 = np.array([0, 160, 160])  # Lower bound for red in HSV
    upper_red1 = np.array([10, 255, 255])  # Upper bound for red in HSV
    lower_red2 = np.array([160, 120, 120])  # Lower bound for red in HSV
    upper_red2 = np.array([180, 255, 255])  # Upper bound for red in HSV
    '''
    OLD SETTINGS
    lower_red1 = np.array([0, 160, 160])  # Lower bound for red in HSV
    upper_red1 = np.array([10, 255, 255])  # Upper bound for red in HSV
    lower_red2 = np.array([160, 120, 120])  # Lower bound for red in HSV
    upper_red2 = np.array([180, 255, 255])  # Upper bound for red in HSV
    '''
     

    mask1 = cv2.inRange(hsv, lower_red1, upper_red1)
    mask2 = cv2.inRange(hsv, lower_red2, upper_red2)
    
    mask = mask1 + mask2
    return mask

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration(sensor={"output_size": (1640, 1232)},  main = {"format" : "RGB888"}))
picamera2.start()

while True:
    image = picamera2.capture_array()
    cv2.imshow("Image",image)

    red_mask = get_red_mask(image)
    red_image = cv2.bitwise_and(image, image, mask=red_mask)        
    gray = cv2.cvtColor(red_image, cv2.COLOR_BGR2GRAY)
    _, thresholded = cv2.threshold(gray, 50, 255, cv2.THRESH_BINARY)
        
    #cv2.imshow("Thresholded Image", thresholded)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break