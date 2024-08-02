import pygame
import comms
import sys

# Init data communication
server_address = "172.22.0.5"
server_port = 6944
SenderSocket = comms.setup_client(server_address, server_port)

# Initalize data dictionary
previous_data = {}
data = {}

# Initialize Pygame
pygame.init()

# Set up the drawing window and title
screen = pygame.display.set_mode([1274, 718])
pygame.display.set_caption("LED Tracker")

# Run until the user asks to quit
running = True


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
    
    # Get the positions from the data dictionary by converting the values to a list
    positions = list(data.values())
    positions = [tuple(pos[0]) for pos in positions]
    positions = [(x, y) for y, x in positions]

    # Draw circles at the positions
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