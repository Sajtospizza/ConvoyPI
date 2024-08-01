import picfuncs as pf
import cv2
import picamera2

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration())
picamera2.start()

# Camera parameters
camera_framerate = 22  # Adjustable
num_frames = 90  

# Frequencies to adjust
led_frequencies = [2] 
frequency_tolerance = 0.5 

# Console output for debugging
print("Detecting LEDs...")
# Initialize positions
led_positions = pf.detect_leds(picamera2, num_frames, camera_framerate, led_frequencies, frequency_tolerance)
print(led_positions)

# Track LEDs
print("Tracking LEDs...")
while True:
    image = picamera2.capture_array()
    
    led_positions = pf.track_leds(image, led_positions)
    
    # Print output for debug
    print(led_positions)

    # Draw average positions
    pf.draw_average_positions(image, led_positions)
    
    cv2.imshow('LED Tracking', image)
    
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

picamera2.stop()
cv2.destroyAllWindows()
