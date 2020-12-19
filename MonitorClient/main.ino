#include <Adafruit_GFX.h>    // Core graphics library
#include <Adafruit_TFTLCD.h> // Hardware-specific library

// Define Pins
#define LCD_CS A3
#define LCD_CD A2
#define LCD_WR A1
#define LCD_RD A0
#define LCD_RESET A4

// Define Colors as CONSTS
#define BLACK   0x0000
#define BLUE    0x001F
#define RED     0xF800
#define GREEN   0x07E0
#define CYAN    0x07FF
#define MAGENTA 0xF81F
#define YELLOW  0xFFE0
#define WHITE   0xFFFF

// Bar Class
#ifndef BAR_H
#define BAR_H

#include <Adafruit_GFX.h>    // Core graphics library
#include <Adafruit_TFTLCD.h> // Hardware-specific library

class Bar
{
  public:
    Bar(Adafruit_TFTLCD screen, int x, int y, int w, int h);
    void draw();
    void setValue(int inp);
    int getValue();
  private:
    float _value;
    bool _visible;
    int _x;
    int _y;
    int _w;
    int _h;
    Adafruit_TFTLCD _screen;
};

Bar::Bar(Adafruit_TFTLCD screen, int x, int y, int w, int h)
{
  _screen = screen;
  _x = x;
  _y = y;
  _w = w;
  _h = h;
  _value = 0.0;
  _visible = true;
}

void Bar::draw()
{
  _screen.fillRect(_x, _y, _h, _w, RED); // Background
  _screen.fillRect(_x, _y, _h, int(_w/_value), GREEN); // Draw Percentage on top.
}

void Bar::setValue(int inp)
{
  _value = inp;
}

int Bar::getValue()
{
  return _value;
}

#endif

// Define Global Variables
Adafruit_TFTLCD screen(LCD_CS, LCD_CD, LCD_WR, LCD_RD, LCD_RESET);

Bar testBar(screen, 50, 50, 10, 200);
int value=50;

void setup(void) {
  // Setup Serial
  Serial.begin(9600);
  Serial.println("User Interface v. 0.1");
  Serial.println("Skelton Networks");
  Serial.println("zskelton@skeltonnetworks.com");
  Serial.println("---------------------");

  // Start Screen
  screen.reset();
  screen.begin(screen.readID());
  screen.fillScreen(BLACK);
  Serial.println("Screen Up.");

  // Initial Value
  testBar.setValue(50);
  screen.fillRect(100, 100, 10, 200, RED);
}

void loop()
{
  // Show Value
  char receiveVal;
  if(Serial.available() > 0)
  {
    receiveVal = Serial.read();
    Serial.print("Received: %c");
    Serial.println(receiveVal);
  }

  // Redraw Bar
  //testBar.draw();
  screen.fillRect(100, 100, 10, 100, GREEN);
}
