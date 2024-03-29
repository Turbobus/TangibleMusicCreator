
// ----------------------------------------------------------- SPI setup -----------------------------------------

#include <SPI.h>
#include <PN532_SPI.h>
#include "PN532.h"
PN532_SPI pn532spi(SPI, 52);
PN532 SPI1(pn532spi);

// ----------------------------------------------------------- HSU setup -----------------------------------------

#include <PN532_HSU.h>      
PN532_HSU pn532hsu1(Serial1);
PN532 HSU1(pn532hsu1);
PN532_HSU pn532hsu2(Serial2);
PN532 HSU2(pn532hsu2);

// ----------------------------------------------------------- I2C setup -----------------------------------------

#include <Wire.h>
#include <PN532_I2C.h>
PN532_I2C pn532i2c(Wire1);
PN532 i2c1(pn532i2c);	

// ----------------------------------------------------------- Variables -----------------------------------------

#define debug false
#define multiRead false
int activeSensor = 0;
uint8_t latestUids[3][7];

// ------------------------------------------------------ Arduino standard functions ------------------------------

void setup(void) {
  Serial.begin(115200);

  if (debug) {
    Serial.println("Hello! Serial has started :)");
  } else {
    sendToComputer("programOn");
  }

  setupNFC(HSU1, "HSU_1");
  //setupNFC(HSU2);
  setupNFC(i2c1, "I2C_1");
  setupNFC(SPI1, "SPI_1");

  println("Ready To Read");
}

void loop(void) {
  readSensor(HSU1);
  activeSensor += 1;
  //readSensor(HSU2);
  readSensor(i2c1);
  activeSensor += 1;
  readSensor(SPI1);
  activeSensor = 0;
}


// ------------------------------------------------------------------------- Sensor setup --------------------------------------

void setupNFC(PN532 nfc, String text){

  nfc.begin();

  uint32_t versiondata = nfc.getFirmwareVersion();
  if (! versiondata) {
    sendToComputer("error");
    print("Didn't find PN53x board"); print(" on: "); println(text);
    while (1); // halt
  }
  
  if (debug) {
    // Got ok data, print it out!
    Serial.print("Found chip PN5"); Serial.print((versiondata>>24) & 0xFF, HEX); Serial.print(" on: "); Serial.println(text); 
    Serial.print("Firmware ver. "); Serial.print((versiondata>>16) & 0xFF, DEC); 
    Serial.print('.'); Serial.println((versiondata>>8) & 0xFF, DEC);
    Serial.println("Waiting for an ISO14443A Card ...");
  }

  nfc.setPassiveActivationRetries(0);
  
  // configure board to read RFID tags
  nfc.SAMConfig();
}

// --------------------------------------------------------------------------------- Sensor reading --------------------------------------

void readSensor(PN532 nfc){
  uint8_t success;
  uint8_t uid[] = { 0, 0, 0, 0, 0, 0, 0 };  // Buffer to store the returned UID
  uint8_t uidLength;                        // Length of the UID (4 or 7 bytes depending on ISO14443A card type)

  // Wait for an ISO14443A type cards (Mifare, etc.).  When one is found
  // 'uid' will be populated with the UID, and uidLength will indicate
  // if the uid is 4 bytes (Mifare Classic) or 7 bytes (Mifare Ultralight)
  success = nfc.readPassiveTargetID(PN532_MIFARE_ISO14443A, uid, &uidLength);
  
  if (success) {

    // Makes it so the same tag can not be read from the same sensor twice in a row
    if(!multiRead && memcmp(uid, latestUids[activeSensor], sizeof(uid)) == 0){
      return;
    } else {
      memcpy(latestUids[activeSensor], uid, sizeof(uid));
    }
    
    if (debug) {
      // Display some basic information about the card
      Serial.println("Found an ISO14443A card");
      Serial.print("  UID Length: ");Serial.print(uidLength, DEC);Serial.println(" bytes");
      Serial.print("  UID Value: ");
      nfc.PrintHex(uid, uidLength);
      Serial.println("");
    }
    
    // Mifare classic reading
    if (uidLength == 4 && debug)
    {
      readMifareClassic(nfc, success, uid, uidLength);
    }
    
    // Mifare UltraLight reading 
    if (uidLength == 7) {
      println("");
      println("Detected a Mifare Ultralight tag (7 byte UID)");
      bool read;
      String result;
      read = readUltralight(&result, nfc);
      if (read) {
        print("Tag Data: ");
        sendToComputer(result);
      } else {
        println("Tag Retrieval Failed");
      }
      //delay(1000);
    } else {
      println("Ooops ... unable to read the requested page!?");
    }
    println("");
  }
}


bool readUltralight(String* result, PN532 nfc) {
    uint8_t entries[8 * 4];

    // Retrieve Raw Data from Tag
    bool successfulRetrieval = getEntriesFromUltralight(entries, nfc);
     
    if (!successfulRetrieval) {
      println("Tag Scan failed, stopping Ultralight read!");
      return false;
    }

    //Convert raw entries to String
    *result = extractDataFromPages(entries);
    return true;
}

bool getEntriesFromUltralight(uint8_t *entries, PN532 nfc) {
  for (int p = 7; p < 15; p++) {
    uint8_t data[4];
    
    bool success = nfc.mifareultralight_ReadPage(p, data);
    
    if (!success) {
      println("Fatal error during read!");
      return false;
    }  
    
    int baseIndex = (p - 7) * 4;
        
    for (int i = 0; i < 4; i++) {
      entries[baseIndex + i] = data[i];
    }
    //delay(10);
  }
  return true;
}

String extractDataFromPages(uint8_t entries[8 * 4]) {
  String result;
  for (int l = 0; l < 32; l++) {
    String hexValue = String(entries[l], HEX);
    if (l == 0 && hexValue == "65") {
      continue;
    } else if (l == 1 && hexValue == "6e") {
      continue;
    } else if (hexValue == "fe") {
      return result;
    } else {
     // Serial.println((char)entries[l]);
      result += ((char)entries[l]);
    }
  }
  return "2";
}

//void copy(uint8_t* src, uint8_t* dst, int len) {
  //memcpy(dst, src, sizeof(src[0]) * len);
//}


//---------------------------------------------------------------------------- Printer helpers-----------------------------------

void sendToComputer(String inputString) {
  // Convert the input string to a character array
  char outputString[inputString.length() + 2];  // +2 for markers

  // Add the starting marker (byte value 2)
  outputString[0] = 2;

  // Copy the input string to the output array starting from index 1
  inputString.toCharArray(outputString + 1, inputString.length() + 1);

  // Add the ending marker (byte value 3)
  outputString[inputString.length() + 1] = 3;

  // Print the modified string to the serial port
  Serial.print(outputString);
}

void print(String text){
  if(debug){
    Serial.print(text);
  }
}

void println(String text){
  if(debug){
    Serial.println(text);
  }
}



// -------------------------------------------------------------------------------------- Mifare Classic ----------------------------

void readMifareClassic(PN532 nfc, uint8_t success, uint8_t *uid, uint8_t uidLength){
  // We probably have a Mifare Classic card ... 
        Serial.println("Seems to be a Mifare Classic card (4 byte UID)");
      
        // Now we need to try to authenticate it for read/write access
        // Try with the factory default KeyA: 0xFF 0xFF 0xFF 0xFF 0xFF 0xFF
        Serial.println("Trying to authenticate block 4 with default KEYA value");
        uint8_t keya[6] = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
      
        // Start with block 4 (the first block of sector 1) since sector 0
        // contains the manufacturer data and it's probably better just
        // to leave it alone unless you know what you're doing
        success = nfc.mifareclassic_AuthenticateBlock(uid, uidLength, 4, 0, keya);
      
        if (success)
        {
          Serial.println("Sector 1 (Blocks 4..7) has been authenticated");
          uint8_t data[16];
      
          // If you want to write something to block 4 to test with, uncomment
          // the following line and this text should be read back in a minute
          // data = { 'a', 'd', 'a', 'f', 'r', 'u', 'i', 't', '.', 'c', 'o', 'm', 0, 0, 0, 0};
          // success = nfc.mifareclassic_WriteDataBlock (4, data);

          // Try to read the contents of block 4

          success = nfc.mifareclassic_ReadDataBlock(4, data);
      
          if (success)
          {
            // Data seems to have been read ... spit it out
            Serial.println("Reading Block 4:");
            nfc.PrintHexChar(data, 16);
            Serial.println("");
        
            // Wait a bit before reading the card again
            //delay(1);
          }
          else
          {
            Serial.println("Ooops ... unable to read the requested block.  Try another key?");
          }
        }
        else
        {
          Serial.println("Ooops ... authentication failed: Try another key?");
        }
}
