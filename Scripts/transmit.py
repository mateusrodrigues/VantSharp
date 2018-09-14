import sys
import time
import serial
from xbee import ZigBee

# count arguments passed in
arg_count = len(sys.argv)

# create device
serial_port = serial.Serial('/dev/ttyUSB0', 115200)
xbee = ZigBee(serial_port, escaped=False)

# get and assemble data to send
DATA = bytes.fromhex(sys.argv[1])

# send data
xbee.send('tx',
        dest_addr_long=b'\x00\x00\x00\x00\x00\x00\xFF\xFF',
        data=DATA)
