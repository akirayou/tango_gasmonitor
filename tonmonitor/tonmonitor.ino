#include "CCS811.h"
#define ADDR      0x5A
#define WAKE_PIN 8 //#dummy
CCS811 sensor;

#define BUZ 10
bool initok=false;
void setup() {
  
  // put your setup code here, to run once:
  pinMode(BUZ,OUTPUT);
  Serial.begin(115200);
  if(sensor.begin(uint8_t(ADDR), uint8_t(WAKE_PIN)))initok=true;
}
unsigned int count=0;
unsigned long lastacc=0;
unsigned long lastclick=0;

int voc=0;
int co2=0;
void loop() {
  
  // put your main code here, to run repeatedly:
  if(! initok)Serial.println(" init failed");
  
  
  unsigned long now=millis();
  if(now-lastacc>250){
    lastacc=now;
    sensor.getData();
    voc=sensor.readTVOC();
    co2=sensor.readCO2();

    /*
    Serial.print(voc);
    Serial.print(" ");
    Serial.println(co2); 
    Serial.println(sensor.readHW_ID());
    */
    count++;
 
  }

  //dummpy voc for test
/*
  voc=count%118;
  voc*=10;
  */  
  //voc to code
  unsigned int code=voc;
  if(code>1000)code=1000;
  code=1000-code;
  
  unsigned long mnow=micros();
  if( mnow-lastclick > (unsigned long)(10*1000 +(unsigned long)code*(1000/12) ) ){
     lastclick=mnow;
  
    digitalWrite(BUZ,HIGH);
    delayMicroseconds(20);
    digitalWrite(BUZ,LOW);    
  }
  
  
}
