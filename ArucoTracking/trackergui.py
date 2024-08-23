import pygame
import comms
import sys

# Init data communication
server_address = "172.22.0.192"
server_port = 6944
SenderSocket = comms.setup_client(server_address, server_port)

# Initalize data dictionaryyy
previous_data = {}
data = {}

# Initialize Pygame
pygame.init()

# Set up the drawing window and title
screen = pygame.display.set_mode([1300, 800])
pygame.display.set_caption("LED Tracker")

# Run until the user asks to quit
running = True

cars = {}

while running:
    # Fill the background with white
    screen.fill((255, 255, 255))

    try:
        # Incoming data
        data = comms.receive_data(SenderSocket, 1024)
        previous_data = data
    except BlockingIOError:
        # No data available, continue without crashing
        data = previous_data
        pass
    
    # Get the positions
    for key, value in data.items():
        cars[key] = value

    positions = list(cars.values())
    positions = [(x, y) for x, y in positions]

    print(positions)
    for pos in positions:
        pygame.draw.circle(screen, (0, 0, 0), pos, 5)
    
    # Flip the display
    pygame.display.flip()    
    
    # Check for quit events
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

# Quit Pygame
pygame.quit()