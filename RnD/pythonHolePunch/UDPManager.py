import socket
import time

print("starting UDP punch")

serverAddressPort = ("192.168.1.40", 11000)
bufferSize = 1024
UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

print("Sending byte")
resp = [0]
responseBytes = bytes(resp)
UDPClientSocket.sendto(responseBytes, serverAddressPort)

msgFromServer = UDPClientSocket.recvfrom(bufferSize)
print(msgFromServer)

enduser = msgFromServer[0].decode("utf-8")
parts = enduser.split(".")
enduserIP = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3]
enduserPort = int(parts[4])
enduserAddressPort = tuple([enduserIP, enduserPort])
print(enduserAddressPort)
print("Established")
time.sleep(1)
responseBytes = bytes("Hello There", "utf-8")
UDPClientSocket.sendto(responseBytes, enduserAddressPort)
msgFromServer = UDPClientSocket.recvfrom(bufferSize)
print("Message: " + str(msgFromServer[0], 'utf-8') + " Sender: " + str(msgFromServer[1]) )
msg = ""
while(True):
    if(msg == ""):
        print("Message: " + str(msgFromServer[0], 'utf-8'))
        responseBytes = bytes("Tock", "utf-8")
        UDPClientSocket.sendto(responseBytes, enduserAddressPort)
    else:
        responseBytes = bytes("Tick", "utf-8")
        UDPClientSocket.sendto(responseBytes, enduserAddressPort)
    msgFromServer = UDPClientSocket.recvfrom(bufferSize)
    msg = str(msgFromServer[0], 'utf-8')
