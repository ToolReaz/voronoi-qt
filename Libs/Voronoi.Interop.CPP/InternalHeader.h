#pragma once
#include <metahost.h>

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