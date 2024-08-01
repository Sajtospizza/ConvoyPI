import cv2
import numpy as np
from collections import deque
from scipy.fft import fft
from picamera2 import Picamera2

def get_red_mask(image):
    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    # Define HSV range for detecting red colors
    lower_red1 = np.array([0, 50, 50])  # Lower bound for red in HSV
    upper_red1 = np.array([10, 255, 255])  # Upper bound for red in HSV
    lower_red2 = np.array([160, 50, 50])  # Lower bound for red in HSV
    upper_red2 = np.array([180, 255, 255])  # Upper bound for red in HSV

    mask1 = cv2.inRange(hsv, lower_red1, upper_red1)
    mask2 = cv2.inRange(hsv, lower_red2, upper_red2)
    
    mask = mask1 + mask2
    return mask

def identify_led_positions(frames, camera_framerate, led_frequencies, frequency_tolerance=0.5):
    num_frames = len(frames)
    fft_frames = fft(frames, axis=0)
    magnitude = np.abs(fft_frames)
    dominant_frequencies = np.argmax(magnitude[1:num_frames//2], axis=0) + 1  # Skip the zero frequency
    frequencies = dominant_frequencies * (camera_framerate / num_frames)
    
    led_positions = {}
    for freq in led_frequencies:
        mask = (frequencies > (freq - frequency_tolerance)) & (frequencies < (freq + frequency_tolerance))
        coords = np.column_stack(np.where(mask))
        led_positions[freq] = coords
    return led_positions

def detect_leds(picamera2, num_frames, camera_framerate, led_frequencies, frequency_tolerance):
    frame_buffer = deque(maxlen=num_frames)
    led_positions = {freq: [] for freq in led_frequencies}
    
    for _ in range(num_frames):
        image = picamera2.capture_array()
        
        red_mask = get_red_mask(image)
        red_image = cv2.bitwise_and(image, image, mask=red_mask)
        
        cv2.imshow("Red Image", red_image)
        
        gray = cv2.cvtColor(red_image, cv2.COLOR_BGR2GRAY)
        
        _, thresholded = cv2.threshold(gray, 50, 255, cv2.THRESH_BINARY)
        
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

def draw_average_positions(frame, led_positions):
    for freq, positions in led_positions.items():
        if positions:
            avg_x = int(np.mean([pos[1] for pos in positions]))
            avg_y = int(np.mean([pos[0] for pos in positions]))
            cv2.circle(frame, (avg_x, avg_y), 5, (0, 255, 0), -1)

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration())
picamera2.start()

# Camera parameters
camera_framerate = 30  # Adjust based on your camera settings
num_frames = 60  # Number of frames to capture for frequency analysis
led_frequencies = [2]  # Define the frequencies of your LEDs
frequency_tolerance = 0.5  # Adjust based on your setup

print("Detecting LEDs...")
led_positions = detect_leds(picamera2, num_frames, camera_framerate, led_frequencies, frequency_tolerance)
print(led_positions)
print("Tracking LEDs...")

# Tracking in the loop
while True:
    image = picamera2.capture_array()
    
    led_positions = track_leds(image, led_positions)
    print(led_positions)

    # Draw average positions
    draw_average_positions(image, led_positions)
    
    cv2.imshow('LED Tracking', image)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

picamera2.stop()
cv2.destroyAllWindows()
