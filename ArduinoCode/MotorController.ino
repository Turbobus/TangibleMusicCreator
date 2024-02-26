#include <Stepper.h>

// Define constants for stepper motor
const int stepsPerRevolution = 200;
const int motorSpeed = 100; // lower speed for clockwise rotation
const int returnSpeed = 180; // higher speed for counter clockwise rotation

// Define pins
const int buttonPin = 7;
const int limitSwitch1Pin = 2;
const int limitSwitch2Pin = 4;
const int motorPins[] = {8, 9, 10, 11};

// Create instance of Stepper motor
Stepper myStepper(stepsPerRevolution, motorPins[0], motorPins[1], motorPins[2], motorPins[3]);

// Define variables to track motor state
bool isMovingClockwise = false;
bool isMovingCounterClockwise = false;

void setup() {
  // Initialize serial communication
  Serial.begin(9600);

  // Set pin modes
  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(limitSwitch1Pin, INPUT_PULLUP);
  pinMode(limitSwitch2Pin, INPUT_PULLUP);
}

void loop() {
  // Read the state of the button
  int buttonState = digitalRead(buttonPin);

  // Check if the button is pressed
  if (buttonState == HIGH) {
    // Button is pressed, start clockwise movement
    Serial.println("Button is pressed");
    startClockwise();
  }

  // Check for limit switch activation
  if (digitalRead(limitSwitch1Pin) == LOW && isMovingCounterClockwise) {
    // Limit switch 1 activated, stop motor
    Serial.println("Limit switch pressed"); 
    stopMotor();
  } else if (digitalRead(limitSwitch2Pin) == LOW && isMovingClockwise) {
    // Limit switch 2 activated during clockwise movement, switch to counter clockwise movement
    Serial.println("Limit switch pressed");
    startCounterClockwise();
  }

  // Move the motor if it's supposed to be moving
  if (isMovingClockwise) {
    myStepper.setSpeed(motorSpeed);
    myStepper.step(1);
  } else if (isMovingCounterClockwise) {
    myStepper.setSpeed(returnSpeed);
    myStepper.step(-1);
  }
}

// Start clockwise movement
void startClockwise() {
  isMovingClockwise = true;
  isMovingCounterClockwise = false;
  Serial.println("Clockwise movement started");
}

// Start counter clockwise movement
void startCounterClockwise() {
  isMovingCounterClockwise = true;
  isMovingClockwise = false;
  Serial.println("Counter clockwise movement started");
}

// Stop motor movement
void stopMotor() {
  isMovingClockwise = false;
  isMovingCounterClockwise = false;
  Serial.println("Motor stopped");
}
