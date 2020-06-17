#include <QApplication>
#include <QPushButton>
#include <QFileDialog>
#include <QObject>
#include <QCoreApplication>
#include "voronoiapp.h"

int main(int argc, char **argv) {
    QApplication app (argc, argv);

    VoronoiApp voronoiApp;
    voronoiApp.show();

    return app.exec();
}
