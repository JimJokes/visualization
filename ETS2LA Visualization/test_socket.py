import multiprocessing
import websockets
import threading
import logging
import asyncio
import json
import time
import os

connected_clients = {}
loop = None

available_channels = [
    {
        "channel": 1,
        "name": "Translation",
        "description": "Current truck position and orientation in the game world.",
    }
]

class WebSocketConnection:
    def __init__(self, websocket):
        self.websocket = websocket
        self.queue = asyncio.Queue()
        self.sender_task = asyncio.create_task(self.send_messages())

    async def send_messages(self):
        try:
            while True:
                message = await self.queue.get()
                await self.websocket.send(message)
        except Exception as e:
            print("Client disconnected due to exception in send_messages.", str(e))


async def server(websocket):
    global connected_clients
    print("Client Connected!")
    connection = WebSocketConnection(websocket)
    connected_clients[websocket] = connection
    print("Number of connected clients: ", len(connected_clients))
    try:
        async for message in websocket:
            pass  # Handle incoming messages if necessary
    except Exception as e:
        print("Client disconnected due to exception.", str(e))
    finally:
        connection.sender_task.cancel()
        del connected_clients[websocket]
        print("Client disconnected. Number of connected clients: ", len(connected_clients))


async def start():
    global loop
    loop = asyncio.get_running_loop()
    async with websockets.serve(server, "localhost", 37522):
        await asyncio.Future()  # run forever

def run_server_thread():
    asyncio.run(start())

def Initialize():
    global TruckSimAPI
    global socket

    socket = threading.Thread(target=run_server_thread)
    socket.start()
    
    print("Visualization sockets waiting for client...")

Initialize()

while True:     
    
    message = [
        {
            "channel": 1,
            "data": {
                "position": [0, 0, 0],
            }
        }
    ]
    
    # Enqueue the message to all connected clients
    for connection in list(connected_clients.values()):
        asyncio.run_coroutine_threadsafe(connection.queue.put(json.dumps(message)), loop)