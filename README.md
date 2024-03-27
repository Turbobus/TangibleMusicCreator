# Tangible music creator (Bebopbox)
The Bebopbox (the tangible artifact) uses NFC readers placed on a movable arm inside the box to read tags placed on top. The tags are palced on the bottom of certain blocks that were created which each represented its own sound. This then simulates a tangible digital audio workstation and can be used in collaborative music creation.

## The software system
The system exists of two arduinos and one computer. One arduino Due is used to connect all the NFC readers to it and therefore is used to read the sensor data coming from the sensors. This is then sent via the USB serial bus to the computer which runs a C# program and decodes the cammands coming from the arduino. The last arduino (Uno) is used to controll the motor which moves the arm back and forth. 

Uses https://github.com/elechouse/PN532 library
