import socket
import time
import GameMessage_pb2

print("starting UDP punch")
ip = input("please input serverIP")

serverAddressPort = (ip, 11000)
bufferSize = 1024
UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

print("Sending byte")
resp = [0]
responseBytes = bytes(resp)
UDPClientSocket.sendto(responseBytes, serverAddressPort)

msgFromServer = UDPClientSocket.recvfrom(bufferSize)

print(msgFromServer)
