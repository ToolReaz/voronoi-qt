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

LIBS += -L$$PWD -lVoronoi.Interop.CPP
