
#include <NfcAdapter.h>
#include <SoftwareSerial.h>
#include <PN532_SWHSU.h>
#include <PN532.h>
SoftwareSerial SWSerial( 12, 13 ); // RX, TX
PN532_SWHSU pn532swhsu( SWSerial );
NfcAdapter nfc = NfcAdapter(pn532swhsu);


void setup(void) {
    Serial.begin(115200);
    Serial.println("NDEF Reader");
    nfc.begin();
}

void loop(void) {
    Serial.println("\nScan a NFC tag\n");
    if (nfc.tagPresent())
    {
        NfcTag tag = nfc.read();
        tag.print();
    }
    delay(10);
}