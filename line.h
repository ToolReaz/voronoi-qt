#ifndef LINE_H
#define LINE_H

#include "point.h"

class Line
{
public:
    Line(Point& a, Point& b);
    Point getPointA();
    Point getPointB();

private:
    Point *pointA;
    Point *pointB;
};

#endif // LINE_H
