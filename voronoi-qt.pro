TEMPLATE = app
TARGET = voronoi-qt

QT = core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

SOURCES += \
    loadassembly.cpp \
    main.cpp \
    voronoiapp.cpp

HEADERS += \
    DataTypes.h \
    loadassembly.h \
    voronoiapp.h

#INCLUDEPATH += C:\Program Files (x86)\Windows Kits\NETFXSDK\4.8\Include\um
INCLUDEPATH += "C:/Program Files (x86)/Windows Kits/NETFXSDK/4.8/Include/um" \
               "C:/Program Files (x86)/Windows Kits/NETFXSDK/4.8/Lib/um/x64"


#INCLUDEPATH += D:/Code/voronoi-qt/dll
#LIBS += -LD:/Code/voronoi-qt/dll -lVoronoi.dll

win32: LIBS += -L"C:/Program Files (x86)/Windows Kits/NETFXSDK/4.8/Lib/um/x64" -lmscoree
