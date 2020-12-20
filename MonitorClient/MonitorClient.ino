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

// Define Global Variables
Adafruit_TFTLCD screen(LCD_CS, LCD_CD, LCD_WR, LCD_RD, LCD_RESET);
struct label {
  int x;
  int y;
  String text;
};
struct bar {
  int x;
  int y;
  int h;
  int w;
  int value;
};

label title;
label lblCpu;
bar barCpu;
label lblMem;
bar barMem;
label lblNet;
label lblNetConnection;
label lblNetIP;
label lblNetBytesReceved;
label lblNetBytesSent;


bar barNet;

// Functions
void drawScreenObjects();
void drawText(label obj, uint16_t color=WHITE, uint8_t size=2);
void drawBar(bar obj, uint16_t outline=WHITE, uint16_t color=GREEN);

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

  // Initial Values
  screen.setRotation(3);
  // Labels - Static
  title = {x: 0, y: 0, text: "Enoch's Computer Monitor"};
  lblCpu = {x: 0, y: 20, text: "CPU:"};
  lblMem = {x: 0, y: 60, text: "Memory:"};
  lblNet = {x: 0, y: 100, text: "Network:"};
  // Labels - Variable
  lblNetConnection = {x: 0, y: 140, text: "Not Connected."};
  lblNetIP = {x: 0, y: 160, text: ""};
  lblNetBytesReceved = {x: 0, y: 180, text: "Received: 0 MBs"};
  lblNetBytesSent = {x: 0, y: 200, text: "Sent: 0 MBs"};
  // Bars - Variable
  barCpu = {x: 0, y: 40, h: 10, w: 100, value: 50};
  barMem = {x: 0, y: 80, h: 10, w: 100, value: 50};
  barNet = {x: 0, y: 120, h: 10, w: 100, value: 50};

  // Draw Static Objects
  drawText(title);
  drawText(lblCpu);
  drawText(lblMem);
  drawText(lblNet);
}

void loop()
{
  // Show Value
  char receiveVal;
  if(Serial.available() > 0)
  {
    receiveVal = Serial.read();
    Serial.print("Received: ");
    Serial.println(receiveVal);
  }

  // Draw Screen
  drawScreenObjects();

  // Pause Loop
  delay(500); // Pause for the remainder of a second.
}

void drawScreenObjects()
{
  drawBar(barCpu);
  drawBar(barMem);
  drawBar(barNet);
  drawText(lblNetConnection);
  drawText(lblNetIP);
  drawText(lblNetBytesReceved);
  drawText(lblNetBytesSent);
}

void drawText(label obj, uint16_t color=WHITE, uint8_t size=2)
{
  screen.setRotation(3);
  screen.setCursor(obj.x, obj.y);
  screen.setTextColor(color);
  screen.setTextSize(size);
  screen.println(obj.text);
}

void drawBar(bar obj, uint16_t outline=WHITE, uint16_t color=GREEN)
{
  screen.setRotation(3);
  screen.drawRect(obj.x, obj.y, obj.w, obj.h, outline);
  screen.fillRect(obj.x, obj.y, int(obj.value*(obj.w/100)), obj.h, color);
}
