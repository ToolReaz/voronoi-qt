#include <QtWidgets>
#include "voronoiapp.h"
#include "line.h"
#include "loadassembly.h"
#include "DataTypes.h"

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

    // Painter & pen
    painter = new QPainter(pixmap);
    pen = new QPen();
    pen->setWidth(2);
    pen->setColor(QColor(255,0,0,255));
    painter->setPen(*pen);
    painter->drawPoint(QPoint(50,50));


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

        QDataStream in(&file);
        in.setVersion(QDataStream::Qt_DefaultCompiledVersion);
        in >> points;

        QMessageBox::information(this, "Points chargés !", "hey");
    }
}

void VoronoiApp::addPoint() {
    int xCord = spinBoxX->value();
    int yCord = spinBoxY->value();

    painter->drawPoint(QPoint(xCord, yCord));
    pixLabel->setPixmap(*pixmap);

    QMessageBox::information(this, "Succes", "Point ajouté !");
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
    QLibrary dll("D:/Code/voronoi-qt/dll/Voronoi.dll");
    dll.load();

    typedef Line* (*FunctionPrototype)(Point*);
    auto function1 = (FunctionPrototype)dll.resolve("GetVoronoi");


    if (function1) {
        QMessageBox::information(this, "Youpi!", "Function1 is alive !!!");
    }
}

void VoronoiApp::useAssembly() {

    loadAssembly = new LoadAssembly();

    // Perform CLR initialization
        CLRPointers ptrs = CLRPointers();
        HRESULT hr =  loadAssembly->InitializeCLR(L"v4.0.30319", &ptrs);

        // Create a list of points
        Point pts[2] = { Point{1, 5}, Point{5, 5} };

        // Create the MMAP file
        MMAPedFile mmap = MMAPedFile();
        LPCWSTR path = L"File.dat";
        hr |= loadAssembly->InitMMAP(path, 1024 * 1024 * sizeof(Line), &mmap);

        // Write the points to the mmap file
        loadAssembly->WriteData(mmap, pts, 2);

        //Load an assembly and call the required function
        if (hr == S_OK) // if CLR is started successfully and MMAP is created successfully
        {
            loadAssembly->CallClr(
                ptrs,
                L"R:\\Voronoi.Interop.dll",
                L"Voronoi.Interop.EntryPoint",
                L"GetVoronoi",
                path);
        }

        // Fetch computed lines from the mmap file
        Line* lines = nullptr;
        const auto lineCount = loadAssembly->ReadData(mmap, &lines);

        // Display the fetched lines coord
        for(auto i = 0; i<lineCount; i++)
        {
            wprintf(L"Line #%d : X1=%f/X2=%f/Y1=%f/Y2=%f\n", i, lines[i].Point1.X, lines[i].Point2.X, lines[i].Point1.Y, lines[i].Point2.Y);
        }

        // Perform CLR cleanup
        loadAssembly->CleanupCLR(ptrs);
}
