#include "line.h"

Line::Line(Point &a, Point &b)
{
    pointA = &a;
    pointB = &b;
}

Point Line::getPointA() {
    return *pointA;
}

Point Line::getPointB() {
    return *pointB;
}


