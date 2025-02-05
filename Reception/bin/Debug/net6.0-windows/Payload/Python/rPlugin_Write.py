import os

class rPlugin_Write:
    def __init__(self, sock) -> None:
        self.sock = sock

    def write(self, b64_data):
        pass

    def run(self):
        cmd = para[0]
        if cmd[0] == 'w':
            self.write(para[1])