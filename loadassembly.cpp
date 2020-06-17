#include "loadassembly.h"
#include <iostream>
#include <metahost.h>
#include <mscoree.h>
#include <_mingw.h>
#include "DataTypes.h"

LoadAssembly::LoadAssembly()
{

}

void LoadAssembly::load() {
    // Perform CLR initialization
        CLRPointers ptrs = CLRPointers();
        HRESULT hr = LoadAssembly::InitializeCLR(L"v4.0.30319", &ptrs);

        // Create a list of points
        Point pts[2] = { Point{1, 5}, Point{5, 5} };

        // Create the MMAP file
        MMAPedFile mmap = MMAPedFile();
        LPCWSTR path = L"File.dat";
        hr |= InitMMAP(path, 1024 * 1024 * sizeof(Line), &mmap);

        // Write the points to the mmap file
        WriteData(mmap, pts, 2);

        //Load an assembly and call the required function
        if (hr == S_OK) // if CLR is started successfully and MMAP is created successfully
        {
            CallClr(
                ptrs,
                L"R:\\Voronoi.Interop.dll",
                L"Voronoi.Interop.EntryPoint",
                L"GetVoronoi",
                path);
        }

        // Fetch computed lines from the mmap file
        Line* lines = nullptr;
        const auto lineCount = ReadData(mmap, &lines);

        // Display the fetched lines coord
        for(auto i = 0; i<lineCount; i++)
        {
            wprintf(L"Line #%d : X1=%f/X2=%f/Y1=%f/Y2=%f\n", i, lines[i].Point1.X, lines[i].Point2.X, lines[i].Point1.Y, lines[i].Point2.Y);
        }

        // Perform CLR cleanup
        CleanupCLR(ptrs);
}



// Initialize the CLR for the current C++ process
HRESULT LoadAssembly::InitializeCLR(const LPCWSTR version, CLRPointers* runtime)
{
    __CRT_UUID_DECL(IID_ICLRMetaHost, 0xD332DB9E, 0xB9B3, 0x4125, 0x82, 0x07, 0xA1, 0x48, 0x84, 0xF5, 0x32, 0x16);
    __CRT_UUID_DECL(CLSID_CLRMetaHost, 0x9280188d, 0xe8e, 0x4867, 0xb3, 0xc, 0x7f, 0xa8, 0x38, 0x84, 0xe8, 0xde);
    __CRT_UUID_DECL(IID_ICLRRuntimeHost, 0x90F1A06C, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02);
    __CRT_UUID_DECL(CLSID_CLRRuntimeHost, 0x90F1A06E, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02);
    __CRT_UUID_DECL(IID_ICLRRuntimeInfo, 0xBD39D1D2, 0xBA2F, 0x486a, 0x89, 0xB0, 0xB4, 0xB0, 0xCB, 0x46, 0x68, 0x91);

    HRESULT hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&(runtime->MetaHost)));
    if (FAILED(hr))
    {
        wprintf(L"CLRCreateInstance failed w/hr 0x%08lx\n", hr);
        return hr;
    }

    // Get the ICLRRuntimeInfo corresponding to a particular CLR version.
    hr = runtime->MetaHost->GetRuntime(version, IID_PPV_ARGS(&(runtime->RuntimeInfo)));
    if (FAILED(hr))
    {
        wprintf(L"ICLRMetaHost::GetRuntime failed w/hr 0x%08lx\n", hr);
        return hr;
    }

    // Load the CLR into the current process and return a runtime interface  pointer.
    hr = runtime->RuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&(runtime->RuntimeHost)));
    if (FAILED(hr))
    {
        wprintf(L"ICLRRuntimeInfo::GetInterface failed w/hr 0x%08lx\n", hr);
        return hr;
    }


    // Start the CLR.
    if (hr == S_OK)
    {
        hr = runtime->RuntimeHost->Start();
        if (FAILED(hr))
        {
            wprintf(L"CLR failed to start w/hr 0x%08lx\n", hr);
            return hr;
        }
    }
    return ERROR_SUCCESS;
}

// Perform the cleanup of the CLR for the current C++ process
void LoadAssembly::CleanupCLR(CLRPointers runtime)
{
    if (runtime.MetaHost)
    {
        runtime.MetaHost->Release();
        runtime.MetaHost = nullptr;
    }
    if (runtime.RuntimeInfo)
    {
        runtime.RuntimeInfo->Release();
        runtime.RuntimeInfo = nullptr;
    }
    if (runtime.RuntimeHost)
    {
        // Please note that after a call to Stop, the CLR cannot be
        // reinitialized into the same process. This step is usually not
        // necessary. You can leave the .NET runtime loaded in your process.
        runtime.RuntimeHost->Release();
        runtime.RuntimeHost = nullptr;
    }
}

// Call a CLR method (static int name(string arg);)
HRESULT LoadAssembly::CallClr(CLRPointers ptrs, const LPCWSTR path, const LPCWSTR className, const LPCWSTR function, const LPCWSTR args)
{
    DWORD dwRet;
    const HRESULT hr = ptrs.RuntimeHost->ExecuteInDefaultAppDomain(path, className, function, args, &dwRet);
    if (FAILED(hr))
    {
        wprintf(L"ExecuteInDefaultAppDomain failed to start w/hr 0x%08lx\n", hr);
        return hr;
    }
    if (FAILED(dwRet))
    {
        wprintf(L"Managed exception occured w/hr 0x%08lx\n", dwRet);
        return dwRet;
    }
    return ERROR_SUCCESS;
}

// Write points to the mmap file
void LoadAssembly::WriteData(const MMAPedFile file, Point* points, const int pointCount)
{
    *static_cast<INT32*>(file.MMAPedArea) = pointCount;
    // Skip to first element
    const auto FileMapping = static_cast<INT32*>(file.MMAPedArea) + 1;
    const auto PointData = reinterpret_cast<Point*>(FileMapping);
    for(auto i = 0; i<pointCount; i++)
    {
        PointData[i] = points[i];
    }
}

// Read lines from the mmap file
int LoadAssembly::ReadData(const MMAPedFile file, Line** lines)
{
    const auto lineCount = *static_cast<INT32*>(file.MMAPedArea);
    const auto FileMapping = static_cast<INT32*>(file.MMAPedArea) + 1;
    *lines = reinterpret_cast<Line*>(FileMapping);
    return lineCount;
}

// Create a mmap file
HRESULT LoadAssembly::InitMMAP(const LPCWSTR path, const SIZE_T size, MMAPedFile* desc)
{
    HANDLE FileMappingHandle;
    LPVOID FileMapping;

    if ((FileMappingHandle = CreateFileMapping(
        INVALID_HANDLE_VALUE,
        nullptr,
        PAGE_READWRITE,
        0,
        size,
        path)) == nullptr)
    {
        return 0x80000000;
    }

    if ((FileMapping = MapViewOfFile(
        FileMappingHandle,
        FILE_MAP_ALL_ACCESS,
        0,
        0,
        size)) == nullptr)
    {
        return 0x80000000;
    }

    desc->FileHandle = FileMappingHandle;
    desc->MMAPedArea = FileMapping;
    return ERROR_SUCCESS;
}
