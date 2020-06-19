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

struct CLRPointers
{
    ICLRMetaHost* MetaHost;
    ICLRRuntimeHost* RuntimeHost;
    ICLRRuntimeInfo* RuntimeInfo;
};

struct MMAPedFile
{
    LPVOID MMAPedArea;
    HANDLE FileHandle;
};