import pygame
import random
import time
import comms 

# Init data
server_address = "172.22.0.5"
server_port = 6944
SenderSocket = comms.setup_client(server_address, server_port)

# Initialize Pygame
pygame.init()

# Set up the drawing window
screen = pygame.display.set_mode([1274, 718])

# Run until the user asks to quit
running = True

while running:
    # Fill the background with white
    screen.fill((255, 255, 255))

    # Simulate incoming positions (replace with your actual data)
    data = comms.receive_data(SenderSocket, 1024)
    #positions = [(x//2, y//2) for x, y in data]
    positions = list(data.values())
    positions = [tuple(sublist) for sublist in positions[0]]
    positions = [(x, y) for y, x in positions]
    # Draw circles at the positions
    for pos in positions:
        pygame.draw.circle(screen, (0, 0, 255), pos, 10)
        print(pos)
    # Flip the display
    pygame.display.flip()    
    
    # Check for quit events
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

# Done! Time to quit.
pygame.quit()