from graphics import *
import time
import random

winDim = 600
win = GraphWin("Board", winDim, winDim)
left = False
corner = False
tick = 0
pos = 0
c = Circle(Point(300, 300), 10)
c.draw(win)
moveX = random.randint(-5, 5)
moveY = random.randint(-5, 5)

moveX = random.randint(-2, 2)
moveY = random.randint(-2, 2)

moveX = 2
moveY = 2

while True:
    if c.__getattribute__("p1").getX() >= 400:
        moveX = random.randint(-2, 0)
    if c.__getattribute__("p1").getX() <= 100:
        moveX = random.randint(0, 2)
    if c.__getattribute__("p1").getY() >= 400:
        moveY = random.randint(-2, 0)
    if c.__getattribute__("p1").getY() <= 100:
        moveY = random.randint(0, 2)
    time.sleep(0.0001)
    c.undraw()
    print("X:")
    print(c.__getattribute__("p1").getX())
    print("Y")
    print(c.__getattribute__("p1").getY())

    c.move(moveX, moveY)
    c.draw(win)


while True:
    if corner:
        print("is corner")
        corner = False
        moveX = random.randint(-5, 5)
        moveY = random.randint(-5, 5)
    if c.__getattribute__("p1").getX() >= 600 or c.__getattribute__("p1").getX() <= 0 or c.__getattribute__("p1").getY() <= 0 or c.__getattribute__("p1").getY() >= 600:
        corner = True
    time.sleep(0.01)
    c.undraw()
    print(c.__getattribute__("p1").getX())
    c.move(moveX, moveY)
    c.draw(win)

while True:
    time.sleep(0.01)
    c.undraw()
    p1 = c.__getattribute__("p1")
    print(c.__getattribute__("p1").getX())
    if c.__getattribute__("p1").getX() >= 600:
        left = True
    if c.__getattribute__("p1").getX() <= 0:
        left = False
    if left:
        c.move(-1, 0)
    else:
        c.move(1, 0)
    c.draw(win)



