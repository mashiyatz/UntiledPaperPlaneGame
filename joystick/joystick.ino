#include <Bounce.h>
#include <Keyboard.h>
  
Bounce button = Bounce(16, 10);

void setup() {
  Serial.begin(9600);
  pinMode(16, INPUT_PULLUP);
}

void loop() {
  button.update();

  if (button.fallingEdge()) {
    Keyboard.press(KEY_SPACE);
  }
         
  if (button.risingEdge()) {
    Keyboard.release(KEY_SPACE);
  }

  int sensor1 = analogRead(A0);
  delay(1);
  int sensor2 = analogRead(A1);



  int yAxis = map(sensor1, 0, 1023, -5, 5);
  int xAxis = map(sensor2, 0, 1023, -5, 5);

  Serial.print(sensor1);
  Serial.print("  ");
  Serial.println(sensor2);

  if (xAxis <= -4) {
    Keyboard.press(KEY_RIGHT);
    Keyboard.release(KEY_RIGHT);
  } else if (xAxis >= 4) {
    Keyboard.press(KEY_LEFT);
    Keyboard.release(KEY_LEFT);
  } else {
    Keyboard.release(KEY_RIGHT);
    Keyboard.release(KEY_LEFT);
  }

  if (yAxis >= 4) {
    Keyboard.press(KEY_UP);
    Keyboard.release(KEY_UP);
  } else if (yAxis <= -4) {
    Keyboard.press(KEY_DOWN);
    Keyboard.release(KEY_DOWN);
  } else {
    Keyboard.release(KEY_UP);
    Keyboard.release(KEY_DOWN);
  }

  delay(15);
}
