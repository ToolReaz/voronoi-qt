#include <iostream>
//#pragma comment(lib, "Voronoi.Interop.CPP.lib")
#include "../Voronoi.Interop.CPP/Voronoi.h"

int main()
{
    Point pts[2] = { { 5, 7 }, {5, 9} };

    Line* lines = nullptr;
    int linesNb = 0;

    fnVoronoiInteropCPP(pts, 2, &lines, &linesNb, L"R:\\Voronoi.Interop.dll");

    for(int i = 0; i < linesNb; i++)
    {
        wprintf(L"Line #%d : X1=%f/X2=%f/Y1=%f/Y2=%f\n", i, lines[i].Point1.X, lines[i].Point2.X, lines[i].Point1.Y, lines[i].Point2.Y);
    }
}