TEMPLATE = app
TARGET = voronoi-qt

QT = core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

SOURCES += \
    main.cpp \
    voronoiapp.cpp

HEADERS += \
    Voronoi.h \
    voronoiapp.h

#LIBS += -L"$$PWD/lib/Voronoi.Interop.CPP.lib"

#LIBS += -LVoronoi.Interop.CPP.lib

#LIBS += "D:/Voronoi.Interop.CPP.lib"

LIBS += -L$$PWD -lVoronoi.Interop.CPP
