TEMPLATE = app
TARGET = voronoi-qt

QT = core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

SOURCES += \
    main.cpp \
    voronoiapp.cpp

HEADERS += \
    DataTypes.h \
    lib/Voronoi.h \
    voronoiapp.h

DISTFILES += \
    lib/C5.dll \
    lib/C5.xml \
    lib/Voronoi.Interop.dll \
    lib/Voronoi.Interop.pdb \
    lib/Voronoi.dll \
    lib/Voronoi.pdb

