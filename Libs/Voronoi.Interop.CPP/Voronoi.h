#pragma once

struct Point {
    double X;
    double Y;
};

struct Line
{
    Point Point1;
    Point Point2;
};

extern "C" _declspec(dllexport) int fnVoronoiInteropCPP(Point* pts, int points_nb, Line** lines, int* lines_nb, const wchar_t* assembly);