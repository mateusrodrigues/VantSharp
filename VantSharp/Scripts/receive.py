import os
import serial
from xbee import ZigBee

serial_port = serial.Serial('/dev/ttyUSB0', 115200)
xbee = ZigBee(serial_port, escaped=False)

if os.path.exists('output.txt'):
    os.remove('output.txt')

while True:
    try:
        frame = xbee.wait_read_frame()
        # print(frame)

        with open('output.txt', 'a+') as f:
            f.write(frame['rf_data'].hex())
    except KeyboardInterrupt:
        break

serial_port.close()