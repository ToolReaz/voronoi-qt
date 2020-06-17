#ifndef VORONOIAPP_H
#define VORONOIAPP_H

#include <QObject>
#include <QWidget>
#include <QMap>
#include <QtCore>
#include <QtWidgets>
#include "loadassembly.h"
#include "DataTypes.h"

QT_BEGIN_NAMESPACE
class QPushButton;
class QLabel;
class QLineEdit;
class QTextEdit;
QT_END_NAMESPACE

class VoronoiApp : public QWidget
{
    Q_OBJECT
public:
    explicit VoronoiApp(QWidget *parent = nullptr);

public slots:
    void loadFromFile();
    void addPoint();
    void saveToFile();
    void generate();
    void useAssembly();

private:
    QHBoxLayout *topBarLayout;
    QVBoxLayout *globalLayout;
    QLabel *labelX;
    QSpinBox *spinBoxX;
    QLabel *labelY;
    QSpinBox *spinBoxY;
    QPushButton *addCordBtn;
    QSpacerItem *horizontalSpacer;
    QPushButton *importFileBtn;
    QPushButton *exportImgBtn;
    QPushButton *generateBtn;

    QMap<QString, QString> points;
    QPixmap *pixmap;
    QLabel *pixLabel;
    QPainter *painter;
    QPen *pen;

    LoadAssembly *loadAssembly;

};

#endif // VORONOIAPP_H
