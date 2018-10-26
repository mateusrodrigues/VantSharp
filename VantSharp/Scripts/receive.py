import serial
from xbee import ZigBee

serial_port = serial.Serial('/dev/ttyUSB0', 115200)
xbee = ZigBee(serial_port, escaped=False)

while True:
    try:
        frame = xbee.wait_read_frame()
        print(frame)
    except KeyboardInterrupt:
        break

serial_port.close()