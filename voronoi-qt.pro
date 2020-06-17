TEMPLATE = app
TARGET = voronoi-qt

QT = core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

SOURCES += \
    line.cpp \
    loadassembly.cpp \
    main.cpp \
    point.cpp \
    voronoiapp.cpp

HEADERS += \
    DataTypes.h \
    line.h \
    loadassembly.h \
    point.h \
    voronoiapp.h

#INCLUDEPATH += C:\Program Files (x86)\Windows Kits\NETFXSDK\4.8\Include\um
INCLUDEPATH += "C:\Program Files (x86)/Windows Kits/NETFXSDK/4.8/Include/um"

#INCLUDEPATH += D:/Code/voronoi-qt/dll
#LIBS += -LD:/Code/voronoi-qt/dll -lVoronoi.dll
