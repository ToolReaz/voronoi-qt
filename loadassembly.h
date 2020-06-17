#ifndef LOADASSEMBLY_H
#define LOADASSEMBLY_H
#include <iostream>
#include <metahost.h>
#include <mscoree.h>
#pragma comment(lib, "mscoree.lib")
#include "DataTypes.h"

class LoadAssembly
{
public:
    LoadAssembly();
    void load();
    void WriteData(const MMAPedFile file, Point* points, const int pointCount);
    int ReadData(const MMAPedFile file, Line** lines);
    HRESULT InitializeCLR(const LPCWSTR version, CLRPointers* ptrs);
    void CleanupCLR(CLRPointers runtime);
    HRESULT InitMMAP(const LPCWSTR path, const SIZE_T size, MMAPedFile* desc);
    HRESULT CallClr(CLRPointers ptrs, const LPCWSTR path, const LPCWSTR className, const LPCWSTR function, const LPCWSTR args);
};

#endif // LOADASSEMBLY_H
