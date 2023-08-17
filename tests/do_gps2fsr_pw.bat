rem was "testit.bat"
nmea2xml\bin\debug\gps2xml c:\temp\pw031102.gps c:\temp\pw031102.xml
fsrtools\bin\debug\xml2fsr c:\temp\pw031102.xml c:\temp\pw031102.fsr
dir c:\temp\pw031102.*

