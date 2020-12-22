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

// In Place of Classes, Using Structs
struct label {
    int x;
    int y;
    String text;
    bool changed;
};
struct bar {
    int x;
    int y;
    int h;
    int w;
    int value;
    bool changed;
};

label title; // Static
label lblCpu; // Static
bar barCpu; // Updates on change
label lblMem; // Static
bar barMem; // Updates on change
label lblNet; // Static
label lblNetConnection; // Updates on change
label lblNetIP; // Updates on change
label lblNetBytesReceived; // Static
label lblNetBytesSent; // Static
bar barNet; // Updates on change

// Functions
void drawScreenObjects();
void drawText(label obj, uint16_t color = WHITE, uint8_t size = 2);
void drawBar(bar obj, uint16_t outline = WHITE, uint16_t color = GREEN);
void eraseText(label obj, uint16_t color = BLACK, uint8_t size = 2);
bool getData();

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
    title = { x: 0, y : 0, text : "Enoch's Computer Monitor", changed : false };
    lblCpu = { x: 0, y : 20, text : "CPU:", changed : false };
    lblMem = { x: 0, y : 60, text : "Memory:", changed : false };
    lblNet = { x: 0, y : 100, text : "Network:", changed : false };
    // Labels - Variable
    lblNetConnection = { x: 0, y : 140, text : "Not Connected." };
    lblNetIP = { x: 0, y : 160, text : "" };
    lblNetBytesReceived = { x: 0, y : 180, text : "Received", changed : false };
    lblNetBytesSent = { x: 0, y : 200, text : "Sent", changed : false };
    // Bars - Variable
    barCpu = { x: 0, y : 40, h : 10, w : 100, value : 0, changed : false };
    barMem = { x: 0, y : 80, h : 10, w : 100, value : 0, changed : false };
    barNet = { x: 0, y : 120, h : 10, w : 100, value : 0, changed : false };

    // Draw Static Objects
    drawText(title);
    drawText(lblCpu);
    drawText(lblMem);
    drawText(lblNet);
    drawText(lblNetBytesReceived);
    drawText(lblNetBytesSent);
    // Draw Frames for Bars
    drawBar(barCpu);
    drawBar(barMem);
    drawBar(barNet);
}

void loop()
{
    if (Serial.available() > 0)
    {
        // Get Data (If none, skip)
        getData();
        // Draw Objects
        drawScreenObjects();
    }
}

bool getData()
{
    // Format:
    // 0 - [barCpu]                  : 0-100
    // 1 - [barMem]                  : 0-100
    // 2 - [barNet]                  : 0|100
    // 3 - [lblNetConnect]           : "" | Type
    // 4 - [lblNetIP]                : "" | IP
    // 5 - [lblNetBytesReceivedData] : Number
    // 6 - [lblNetBytesSentData]       Number
    // End \n
    // Example
    // 50:50:100:Ethernet:192.168.1.10:Received          10 MBs:Sent             100 MBs

    // Parse Variables
    String bCpu = Serial.readStringUntil(':');
    String bMem = Serial.readStringUntil(':');
    String bNet = Serial.readStringUntil(':');
    String lCon = Serial.readStringUntil(':');
    String lIP = Serial.readStringUntil(':');
    String lRec = Serial.readStringUntil(':');
    String lSen = Serial.readStringUntil('\n');

    // Check for New Bar Values
    if (bCpu.toInt() != barCpu.value) {
        barCpu.value = bCpu.toInt();
        barCpu.changed = true;
    }
    if (bMem.toInt() != barMem.value) {
        barMem.value = bMem.toInt();
        barMem.changed = true;
    }
    if (bNet.toInt() != barNet.value) {
        barNet.value = bNet.toInt();
        barNet.changed = true;
    }

    // Check for New Text Values
    if (lCon != lblNetConnection.text) {
        lblNetConnection.text = lCon;
        lblNetConnection.changed = true;
    }
    if (lIP != lblNetIP.text) {
        lblNetIP.text = lIP;
        lblNetIP.changed = true;
    }
    if (lRec != lblNetBytesReceived.text) {
        lblNetBytesReceived.text = lRec;
        lblNetBytesReceived.changed = true;
    }
    if (lSen != lblNetBytesSent.text) {
        lblNetBytesSent.text = lSen;
        lblNetBytesSent.changed = true;
    }
}

void drawScreenObjects() // (String bars[], String texts[])
{
    // Update CPU, if new.
    if (barCpu.changed == true) {
        drawBar(barCpu);
        barCpu.changed = false;
    }

    // Update Memory, if new.
    if (barMem.changed == true) {
        drawBar(barMem);
        barMem.changed = false;
    }

    // Update Network Connection, if new.
    if (barNet.changed == true) {
        drawBar(barNet);
        barNet.changed = false;
    }

    // Update Connection, if new.
    if (lblNetConnection.changed == true) {
        drawText(lblNetConnection);
        lblNetConnection.changed = false;
    }

    // Update IP, if new.
    if (lblNetIP.changed == true) {
        drawText(lblNetIP);
        lblNetIP.changed = false;
    }

    // Update Received, if new.
    if (lblNetBytesReceived.changed == true) {
        drawText(lblNetBytesReceived);
        lblNetBytesReceived.changed = false;
    }

    // Update Sent, if new.
    if (lblNetBytesSent.changed == true) {
        drawText(lblNetBytesSent);
        lblNetBytesSent.changed = false;
    }
}

void eraseText(label obj, uint16_t color = BLACK, uint8_t size = 2)
{
    int16_t  xb, yb;
    uint16_t wb, hb;
    screen.getTextBounds(obj.text, obj.x, obj.y, &xb, &yb, &wb, &hb);
    screen.fillRect(xb, yb, wb, hb, color);
}

void drawText(label obj, uint16_t color = WHITE, uint8_t size = 2)
{
    eraseText(obj);
    screen.setRotation(3);
    screen.setCursor(obj.x, obj.y);
    screen.setTextColor(color);
    screen.setTextSize(size);
    screen.println(obj.text);
}

void drawBar(bar obj, uint16_t outline = WHITE, uint16_t color = GREEN)
{
    screen.setRotation(3);
    screen.fillRect(obj.x, obj.y, obj.w, obj.h, BLACK);
    screen.drawRect(obj.x, obj.y, obj.w, obj.h, outline);
    screen.fillRect(obj.x, obj.y, obj.value, obj.h, color);
}
