import cv2
import numpy as np
from picamera2 import Picamera2

def rgb_to_hsv(rgb):
    rgb = np.uint8([[rgb]])
    hsv = cv2.cvtColor(rgb, cv2.COLOR_RGB2HSV)
    return hsv[0][0]

def get_color_mask_with_tolerance(image, target_rgb, tolerance=20):
    # Convert the target RGB color to HSV
    target_hsv = rgb_to_hsv(target_rgb)
    
    # Define the lower and upper bounds with tolerance
    lower_bound = np.array([
        max(target_hsv[0] - tolerance, 0),
        max(target_hsv[1] - tolerance, 0),
        max(target_hsv[2] - tolerance, 0)
    ], dtype=np.uint8)
    
    upper_bound = np.array([
        min(target_hsv[0] + tolerance, 179),
        min(target_hsv[1] + tolerance, 255),
        min(target_hsv[2] + tolerance, 255)
    ], dtype=np.uint8)

    # Convert the image to HSV
    hsv_image = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    
    # Create the mask for the HSV range
    mask = cv2.inRange(hsv_image, lower_bound, upper_bound)
    
    return mask

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration(sensor={"output_size": (1640, 1232)},  main = {"format" : "RGB888"}))
picamera2.start()

while True:
    image = picamera2.capture_array()

    target_rgb = [110, 30, 105]
    mask = get_color_mask_with_tolerance(image, target_rgb)
    masked_image = cv2.bitwise_and(image, image, mask=mask)
    gray = cv2.cvtColor(masked_image, cv2.COLOR_BGR2GRAY)
    _, thresholded = cv2.threshold(gray, 50, 255, cv2.THRESH_BINARY)
        
    cv2.imshow("Thresholded Image", thresholded)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
