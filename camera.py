import sys
import time
from picamera2 import Picamera2, MappedArray
import cv2
import numpy as np

# Camera setup
def setup_camera():
    picam2 = Picamera2()
    def preview(request):
                with MappedArray(request, "main") as m:
                        pass
    picam2.pre_callback = preview
    picam2.video_configuration.controls.FrameRate = 24.0
    picam2.configure("video")
    picam2.start(show_preview=True)
    return picam2

# Function to perform FFT analysis and identify LED positions
def identify_led_positions(frames, camera_framerate, led_frequencies, frequency_tolerance=0.5):
    num_frames = len(frames)
    fft_frames = np.fft.fft(frames, axis=0)
    magnitude = np.abs(fft_frames)
    dominant_frequencies = np.argmax(magnitude[1:num_frames//2], axis=0) + 1  # Skip the zero frequency
    frequencies = dominant_frequencies * (camera_framerate / num_frames)
    
    led_positions = {}
    for freq in led_frequencies:
        mask = (frequencies > (freq - frequency_tolerance)) & (frequencies < (freq + frequency_tolerance))
        coords = np.column_stack(np.where(mask))
        led_positions[freq] = coords
    return led_positions

camera = setup_camera()
# Allow the camera to warm up
time.sleep(0.1)

# Number of frames to capture in each window
num_frames = 120  # Adjust based on the frequencies you're detecting

# Define frequency ranges for each LED (example: 1 Hz, 2 Hz, 3 Hz)
led_frequencies = [1, 2, 3]
frequency_tolerance = 0.5  # Adjust based on your setup

# Buffer to hold the frames for FFT analysis
frame_buffer = []


threshold = 120

prevframe = camera.capture_array()
prevgray = cv2.cvtColor(prevframe, cv2.COLOR_BGR2GRAY)
cv2.imshow("frame", prevframe)
while True:
    frame = camera.capture_array()
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    frame_diff = cv2.absdiff(gray, prevgray)
    _, thresh = cv2.threshold(frame_diff, threshold, 255, cv2.THRESH_BINARY)
    
    # Find contours in the thresholded image
    contours, _ = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    if contours:
        # Find the largest contour
        largest_contour = max(contours, key=cv2.contourArea)

        # Calculate the centroid of the largest contour
        M = cv2.moments(largest_contour)
        if M["m00"] != 0:
            cX = int(M["m10"] / M["m00"])
            cY = int(M["m01"] / M["m00"])
                
            # Draw a dot at the centroid
            cv2.circle(frame, (cX, cY), 5, (0, 0, 255), -1)

        non_zero_count = np.count_nonzero(thresh)
        total_count = thresh.size
        change_ratio = non_zero_count / total_count

        if change_ratio > 0.1:  # ratio
                print(f"Flash detected at ({cX}, {cY})")
    cv2.imshow('Frame', frame)
    cv2.imshow('Threshold', thresh)
    prevgray = gray
    

