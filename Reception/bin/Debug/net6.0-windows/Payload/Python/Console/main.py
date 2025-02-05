from tools import interactive
from tools import title
from tools import Reception as rp
import os
import argparse
import socket

parser = argparse.ArgumentParser()

# CLIENT (OPTIONAL)
parser.add_argument('--ip', help='Reception server IP')
parser.add_argument('--port', help='Reception server port', type=int)
parser.add_argument('-t', '--timeout', help='Connection timeout(seconds)', default=10, type=int)
parser.add_argument('-p', '--password', help='Reception server password')

# FOR SERVER GUI PROGRAM ONLY.
parser.add_argument('--reception', help='Display mode for Reception server', action='store_true', default=False)

args = parser.parse_args()

def show_title():
    title.show_title()

def reception():
    server_ip = args.ip
    server_port = args.port

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.settimeout(args.timeout)
    s.connect((server_ip, server_port))

    s.send(rp.buffer(3, 0, 'hola')) # SEND KNOCK

    recv = s.recv(6) # HEADER
    # MESSAGE HEADER
    cmd = recv[0]
    param = recv[1]
    _len_bytes = recv[2:6]
    _len = int.from_bytes(_len_bytes, byteorder='little', signed=False)

    # GET MESSAGE
    recv += s.recv(_len)
    if cmd == 1:
        pass

def main():
    # SHOW TITLE
    show_title()

    if args.ip and args.port:
        reception()

    # DO INTERACTIVE
    inter = interactive.MyCmd()
    inter.args = args
    inter.module_dir = os.path.join(os.getcwd(), 'module')
    inter.run()

if __name__ == '__main__':
    main()