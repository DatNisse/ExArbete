from graphics import *
# from udpmanager import *
import socket
import time
import GameMessage_pb2

print("starting UDP punch")
ip = input("please input serverIP")
PlayerName = input("please input your player name")
serverAddressPort = (ip, 11000)
bufferSize = 1024
UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

print("Sending byte")
message = GameMessage_pb2.GameMessage()
message.playerId = PlayerName
message.isActive = False
message.isGreet = True

print(message)

pb = message.SerializeToString()

#resp = [0]
responseBytes = bytes(pb)
UDPClientSocket.sendto(responseBytes, serverAddressPort)

msgFromServer = UDPClientSocket.recvfrom(bufferSize)

print(msgFromServer)


print("Waiting for opponent...")

gamestart = True
playerTurn = False
while gamestart:
    msgFromServer = UDPClientSocket.recvfrom(bufferSize)
    print("Message from server")
    message.ParseFromString(msgFromServer[0])
    print(message)
    if message.move == "newgame":
        gamestart = False


print("opponent name: " + message.playerId)
if message.move_number == -1:
    playerTurn = True

input("waiting for you <3")


game = True
player = 1
moves = 0
winDim = 600
win = GraphWin("Board", winDim, winDim)



class Box:
    is_empty = True
    player_num = False

    def __init__(self, xpos, ypos):
        self.xpos = xpos
        self.ypos = ypos

    def draw_cross(self):
        self.player_num = 1
        self.is_empty = False
        line = Line(Point(self.xpos[0], self.ypos[0]), Point(self.xpos[1], self.ypos[1]))
        line.setWidth(winDim/90)
        line.draw(win)
        line = Line(Point(self.xpos[1], self.ypos[0]), Point(self.xpos[0], self.ypos[1]))
        line.setWidth(winDim/90)
        line.draw(win)

    def draw_circle(self):
        self.player_num = 2
        self.is_empty = False
        circle = Circle(Point((self.xpos[1] + self.xpos[0])/2, (self.ypos[1] + self.ypos[0])/2), winDim/9)
        circle.setWidth(winDim/90)
        circle.draw(win)


board_boxes = []
#var = [[30, 70], [30, 70], Box]
#box = Box([30, 70], [30, 70])
#board_boxes.append(box)

print("hello world")
print(board_boxes)


def draw_board():
    line = Line(Point(20, (winDim/3)), Point(winDim - 20, (winDim/3)))
    line.setWidth(winDim/40)
    line.draw(win)
    line = Line(Point(20, (winDim/3)*2), Point(winDim - 20, (winDim/3)*2))
    line.setWidth(winDim/40)
    line.draw(win)

    line = Line(Point((winDim / 3), 20), Point((winDim / 3), winDim - 20))
    line.setWidth(winDim/40)
    line.draw(win)
    line = Line(Point((winDim / 3) * 2, 20), Point((winDim / 3) * 2, winDim - 20))
    line.setWidth(winDim/40)
    line.draw(win)

    i = 0
    while i < 3:
        j = 0
        while j < 3:
            box = Box([winDim/12 + (winDim/3)*j, winDim/4 + (winDim/3)*j], [winDim/12 + (winDim/3)*i, winDim/4 + (winDim/3)*i])
            board_boxes.append(box)
            j += 1
        i += 1


def check_board(player):
    playfield = []
    for obj in board_boxes:
        playfield.append(obj.player_num)

    i = 0
    while i < 7:
        if playfield[i+0] == player and playfield[i+1] == player and playfield[i+2] == player:  # Checks horiz lines
            return "Player: " + str(player) + " has won!"
        i += 3

    i = 0
    while i < 3:
        if playfield[i+0] == player and playfield[i+3] == player and playfield[i+6] == player:  # Checks vert lines
            return "Player: " + str(player) + " has won!"
        i += 1

    if playfield[0] == player and playfield[4] == player and playfield[8] == player:  # Checks vert lines
        return "Player: " + str(player) + " has won!"
    if playfield[2] == player and playfield[4] == player and playfield[6] == player:  # Checks vert lines
        return "Player: " + str(player) + " has won!"
    return False


draw_board()
for obj in board_boxes:
    print(obj.xpos)
    print(obj.ypos)
legalmove = False
while game:
    win_state = False
    if playerTurn:
        while legalmove:
            mouse = win.getMouse()
            print(mouse)
            boxnum = 0
            for box in board_boxes:
                if box.xpos[0] < mouse.x < box.xpos[1] and box.ypos[0] < mouse.y < box.ypos[1] and box.is_empty:
                    legalmove = True
                    if player == 1:
                        box.draw_cross()
                        win_state = check_board(player)
                        message = GameMessage_pb2.GameMessage()
                        message.playerId = PlayerName
                        message.move = str(boxnum)
                        message.move_number = moves
                        message.isActive = True
                        message.isGreet = False
                        if win_state:
                            message.isGameover = True
                        else:
                            message.isGameover = win_state
                        player = 2
                        moves += 1
                    else:
                        box.draw_circle()
                        win_state = check_board(player)
                        message = GameMessage_pb2.GameMessage()
                        message.playerId = PlayerName
                        message.move = str(boxnum)
                        message.move_number = moves
                        message.isActive = True
                        message.isGreet = False
                        if win_state:
                            message.isGameover = True
                        else:
                            message.isGameover = win_state
                        player = 1
                        moves += 1

                boxnum = boxnum + 1
            pb = message.SerializeToString()
            responseBytes = bytes(pb)
            UDPClientSocket.sendto(responseBytes, serverAddressPort)
            playerTurn = False

    else:
        print("waiting for opponent to make a move")
        msgFromServer = UDPClientSocket.recvfrom(bufferSize)
        print("received move from opponent")
        message.ParseFromString(msgFromServer[0])
        print(message)
        if player == 1:
            board_boxes[int(message.move)].draw_cross()
            win_state = check_board(player)
            player = 2
            moves += 1
        else:
            board_boxes[int(message.move)].draw_circle()
            win_state = check_board(player)
            player = 1
            moves += 1
        playerTurn = True

    if win_state:
        label = Text(Point(winDim / 2, winDim / 2), win_state)
        label.setSize(36)
        label.setTextColor('red')
        label.draw(win)
        win.getMouse()
        game = False
    else:
        if moves == 9:
            label = Text(Point(winDim / 2, winDim / 2), "DRAW!")
            label.setSize(36)
            label.draw(win)
            win.getMouse()
            game = False
win.close()


