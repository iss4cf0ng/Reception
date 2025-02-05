import base64
import struct
from . import pure_AES

def AES_CBC_en(plaintext:str, key:bytes, iv:bytes):
    plaintext = plaintext.encode('utf-8')
    encrypted = pure_AES.AES(key).encrypt_cbc(plaintext, iv)
    return base64.b64encode(encrypted).decode('utf-8')

def AES_CBC_de(cipher:str, key:bytes, iv:bytes):
    pass

def build_buffer(command:int, param:int, buf:bytes) -> bytes:
    command = command.to_bytes(1, 'big')
    param = param.to_bytes(1, 'big')
    _len = len(buf)
    len_bytes = struct.pack('>I', _len)[::-1]
    
    return command + param + len_bytes + buffer

def buffer(command:int, param:int, payload:str) -> bytes:
    data = None
    if isinstance(payload, str):
        data = base64.b64encode(payload.encode('utf-8'))
    elif isinstance(payload, bytes):
        data = payload

    return build_buffer(command, param, data)