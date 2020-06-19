#include <QtWidgets>
#include <iostream>
#include <Voronoi.h>
#include <voronoiapp.h>

VoronoiApp::VoronoiApp(QWidget *parent) : QWidget(parent)
{
    // Layout
    // Top bar layout
    topBarLayout = new QHBoxLayout();
    topBarLayout->setSpacing(10);
    topBarLayout->setObjectName(QString::fromUtf8("topBarLayout"));
    topBarLayout->setContentsMargins(0, 0, 0, 0);

    labelX = new QLabel();
    labelX->setObjectName(QString::fromUtf8("labelX"));
    labelX->setText("X:");

    topBarLayout->addWidget(labelX);

    spinBoxX = new QSpinBox();
    spinBoxX->setObjectName(QString::fromUtf8("spinBoxX"));
    spinBoxX->setFixedWidth(60);
    spinBoxX->setRange(0,780);

    topBarLayout->addWidget(spinBoxX);

    labelY = new QLabel();
    labelY->setObjectName(QString::fromUtf8("labelY"));
    labelY->setText("Y:");

    topBarLayout->addWidget(labelY);

    spinBoxY = new QSpinBox();
    spinBoxY->setObjectName(QString::fromUtf8("spinBoxY"));
    spinBoxY->setFixedWidth(60);
    spinBoxY->setRange(0,520);

    topBarLayout->addWidget(spinBoxY);

    addCordBtn = new QPushButton();
    addCordBtn->setObjectName(QString::fromUtf8("addCordBtn"));
    addCordBtn->setText("Ajouter");

    topBarLayout->addWidget(addCordBtn);

    horizontalSpacer = new QSpacerItem(40, 20, QSizePolicy::Expanding, QSizePolicy::Minimum);

    topBarLayout->addItem(horizontalSpacer);

    flushBtn = new QPushButton();
    flushBtn->setObjectName(QString::fromUtf8("flushBtn"));
    flushBtn->setText("Effacer");

    topBarLayout->addWidget(flushBtn);

    generateBtn = new QPushButton();
    generateBtn->setObjectName(QString::fromUtf8("generateBtn"));
    generateBtn->setText("Générer");

    topBarLayout->addWidget(generateBtn);

    importFileBtn = new QPushButton();
    importFileBtn->setObjectName(QString::fromUtf8("importFileBtn"));
    importFileBtn->setText("Importer fichier de points");

    topBarLayout->addWidget(importFileBtn);

    exportImgBtn = new QPushButton();
    exportImgBtn->setObjectName(QString::fromUtf8("exportImgBtn"));
    exportImgBtn->setText("Exporter en image");

    topBarLayout->addWidget(exportImgBtn);


    // Pixmap
    pixmap = new QPixmap(780,520);
    pixmap->fill(QColor(220,220,220,255));

    // Pens
    pointPen = new QPen();
    pointPen->setWidth(8);
    pointPen->setColor(QColor(56,170,88,255));
    linePen = new QPen();
    linePen->setWidth(2);
    linePen->setColor(QColor(66,114,245,255));

    // Painter
    painter = new QPainter(pixmap);
    painter->setPen(*pointPen);
    //painter->drawPoint(QPoint(50,50));


    // Add pixmap to label after drawing
    pixLabel = new QLabel();
    pixLabel->setPixmap(*pixmap);
    pixLabel->setScaledContents(true);


    // Window settings
    globalLayout = new QVBoxLayout();
    globalLayout->addItem(topBarLayout);

    globalLayout->addWidget(pixLabel);

    setLayout(globalLayout);
    setFixedSize(800, 600);
    setWindowTitle("Voronoi QT");

    // Connect signals to slots
    connect(importFileBtn, &QPushButton::clicked, this, &VoronoiApp::loadFromFile);
    connect(addCordBtn, &QPushButton::clicked, this, &VoronoiApp::addPoint);
    connect(exportImgBtn, &QPushButton::clicked, this, &VoronoiApp::saveToFile);
    connect(generateBtn, &QPushButton::clicked, this, &VoronoiApp::generate);
    connect(flushBtn, &QPushButton::clicked, this, &VoronoiApp::flush);

    // Create point list
    pointList = new QList<Point>();
}



void VoronoiApp::loadFromFile() {
    QString fileName = QFileDialog::getOpenFileName(this, "Open existing map", "", "Text file (*.txt);; All Files (*)");

    if (fileName.isEmpty()) {
        return;
    } else {
        QFile file(fileName);

        if (!file.open(QIODevice::ReadOnly)) {
            QMessageBox::warning(this, "Impossible d'ouvrir le fichier !", file.errorString());
            return;
        }

        painter->setPen(*pointPen);

        QTextStream in(&file);
        while(!in.atEnd()) {
            QString line = in.readLine();
            QStringList cords = line.split(",");
            painter->drawPoint(QPoint(cords[0].toInt(), cords[1].toInt()));
            pointList->append({cords[0].toDouble(), cords[1].toDouble()});
        }

        pixLabel->setPixmap(*pixmap);

        QMessageBox::information(this, "Points chargés !", "hey");
    }
}

void VoronoiApp::addPoint() {
    int xCord = spinBoxX->value();
    int yCord = spinBoxY->value();

    painter->setPen(*pointPen);
    painter->drawPoint(QPoint(xCord, yCord));
    pixLabel->setPixmap(*pixmap);

    pointList->append({(double)xCord, (double)yCord});
}

void VoronoiApp::saveToFile() {
    QString fileName = QFileDialog::getSaveFileName(this, "Save as image", "", "Image (.png)");

    if (fileName.isEmpty()) {
            return;
    } else {
            QFile file(fileName + ".png");
            file.open(QIODevice::WriteOnly);
            pixmap->save(&file, "PNG");

            QMessageBox::information(this, "Succès", "Fichier sauvegardé !");
    }
}

void VoronoiApp::generate() {
        VoronoiApp::clear();

        QLibrary myLib("Voronoi.Interop.CPP.dll");
        typedef void (*MyPrototype)(Point* pts, int points_nb, Line** lines, int* lines_nb, const wchar_t* assembly);
        MyPrototype myFunction = (MyPrototype) myLib.resolve("fnVoronoiInteropCPP");
        if (myFunction) {
            //Point pts[2] = { { 50, 70 }, {150, 70} };
            Point pts[pointList->length()];
            for(int i=0; i < pointList->length(); i++) {
                pts[i] = pointList->at(i);
            }
            Line* lines = nullptr;
            int linesNb = 0;

            /*
            QString pwd = QDir::currentPath();
            QMessageBox::information(this, "path!", pwd);
            pwd.append("Voronoi.Interop.dll");
            wchar_t w[pwd.length()];
            pwd.toWCharArray(w);
            */

            myFunction(pts, pointList->length(), &lines, &linesNb, L"R:\\Voronoi.Interop.dll");
            painter->setPen(*linePen);
            for(int i = 0; i < linesNb; i++)
            {
                //wprintf(L"Line #%d : X1=%f/X2=%f/Y1=%f/Y2=%f\n", i, lines[i].Point1.X, lines[i].Point2.X, lines[i].Point1.Y, lines[i].Point2.Y);
                painter->drawLine((int)lines[i].Point1.X, (int)lines[i].Point1.Y, (int)lines[i].Point2.X, (int)lines[i].Point2.Y);
            }
            pixLabel->setPixmap(*pixmap);
            QMessageBox::information(this, "Succès!", "Génération réussie");
        } else  {
            QMessageBox::warning(this, "Erreur !", "Erreur de chargement de la DLL");
        }
}

void VoronoiApp::clear() {

    // Free old pixmap & painter
    free(pixmap);
    free(painter);

    // Recreate pixmap
    pixmap = new QPixmap(780,520);
    pixmap->fill(QColor(220,220,220,255));
    // Reassign painter
    painter = new QPainter(pixmap);
    painter->setPen(*pointPen);

    // Redraw points
    for(int i=0; i < pointList->length(); i++) {
        painter->drawPoint(QPoint((int)pointList->at(i).X, (int)pointList->at(i).Y));
    }

    // Redraw pixmap
    pixLabel->setPixmap(*pixmap);
}

void VoronoiApp::flush() {

    QMessageBox::StandardButton confirm = QMessageBox::question(this, "Confirmation", "Ceci supprimera les points placés. Êtes vous sûr ?", QMessageBox::Yes | QMessageBox::No);

    if (confirm == QMessageBox::Yes) {
        free(pointList);
        pointList = new QList<Point>();
        VoronoiApp::clear();
    }
}
