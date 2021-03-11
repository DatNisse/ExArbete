from graphics import *
import time
import random

winDim = 600
win = GraphWin("Board", winDim, winDim)
left = False
tick = 0
pos = 0
c = Circle(Point(300, 300), 10)
c.draw(win)

while True:
    time.sleep(0.01)
    c.undraw()
    print(c.__getattribute__("p1").getX())
    c.move(random.randint(-5, 5), random.randint(-5, 5))
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



