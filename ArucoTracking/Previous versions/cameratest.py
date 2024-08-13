import time
import numpy as np
import cv2
from collections import deque
from scipy.fftpack import fft
import threading

# Function to perform FFT analysis and identify LED positions
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

# Function to create a strict mask for the color red
def get_strict_red_mask(image):
    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    
    # Define a stricter range for read
    lower_red1 = np.array([0, 150, 150])
    upper_red1 = np.array([10, 255, 255])
    mask1 = cv2.inRange(hsv, lower_red1, upper_red1)
    
    lower_red2 = np.array([170, 150, 150])
    upper_red2 = np.array([180, 255, 255])
    mask2 = cv2.inRange(hsv, lower_red2, upper_red2)
    
    return mask1 + mask2

# Initialize the webcam
cap = cv2.VideoCapture(0)  # 0 is the default camera

# Check if the webcam is opened correctly
if not cap.isOpened():
    print("Error: Could not open webcam.")
    exit()

# Camera parameters
camera_framerate = 24  # Adjust based on your camera settings

# Number of frames to capture in each window
num_frames = 24  # Adjust based on the frequencies you're detecting

# Define frequency ranges for each LED (example: 1 Hz, 2 Hz, 3 Hz)
led_frequencies = [3]
frequency_tolerance = 0.5  # Adjust based on your setup

# Buffer to hold the frames for FFT analysis
frame_buffer = deque(maxlen=num_frames)

# Thread-safe variables
buffer_lock = threading.Lock()
processed_image = None

# Capture frames continuously
def capture_frames():
    global processed_image
    while True:
        ret, image = cap.read()
        if not ret:
            continue
        
        # Apply Gaussian blur to reduce noise
        blurred_image = cv2.GaussianBlur(image, (5, 5), 0)
        
        red_mask = get_strict_red_mask(blurred_image)
        red_image = cv2.bitwise_and(blurred_image, blurred_image, mask=red_mask)
        gray = cv2.cvtColor(red_image, cv2.COLOR_BGR2GRAY)
        
        # Adjust the threshold to be more selective
        _, thresholded = cv2.threshold(gray, 100, 255, cv2.THRESH_BINARY)
        
        with buffer_lock:
            frame_buffer.append(thresholded)
            if len(frame_buffer) >= num_frames:
                led_positions = identify_led_positions(np.array(frame_buffer), camera_framerate, led_frequencies, frequency_tolerance)
                processed_image = image.copy()
                for freq, coords in led_positions.items():
                    for coord in coords:
                        cv2.circle(processed_image, (coord[1], coord[0]), 5, (255, 0, 0), -1)

# Start frame capture in a separate thread
capture_thread = threading.Thread(target=capture_frames)
capture_thread.start()

# Display frames
while True:
    with buffer_lock:
        if processed_image is not None:
            cv2.imshow("Frame", processed_image)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Cleanup
cap.release()
cv2.destroyAllWindows()
