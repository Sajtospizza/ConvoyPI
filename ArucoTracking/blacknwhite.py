from picamera2 import Picamera2
import cv2

# Initialize the camera
print("Starting camera...")
picamera2 = Picamera2()
picamera2.configure(picamera2.create_video_configuration(sensor={"output_size": (1640, 1232)},  main = {"format" : "RGB888"}))
picamera2.start()

while True:
    image = picamera2.capture_array()

    image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    threshold_value = 105
    max_value = 255
    _, binary_image = cv2.threshold(image, threshold_value, max_value, cv2.THRESH_BINARY)

    cv2.imshow("Image",binary_image)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
picamera2.stop()
cv2.destroyAllWindows()