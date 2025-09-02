# Tangible music creator (Bebopbox)
The Bebopbox uses NFC readers placed on a movable arm inside the box to read tags placed on top. The tags are placed on the bottom of blocks that were created which each represents its own sound. Then, when the arm moves, it plays the sounds of the blocks on top to simulate a digital audio workstation and can be used in collaborative music creation.

## The software system
The system consists of two Arduinos and one computer. One arduino Due is used to connect all the NFC readers to it and therefore is used to read the sensor data coming from the sensors. This is then sent via the USB serial bus to the computer which runs a C# program and decodes the commands coming from the arduino. The last arduino (Uno) is used to control the motor which moves the arm back and forth.

Uses https://github.com/elechouse/PN532 library
