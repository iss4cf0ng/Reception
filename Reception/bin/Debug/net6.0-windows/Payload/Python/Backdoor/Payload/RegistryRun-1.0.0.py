import platform
import winreg
import os

plat = platform.platform().lower()
unix_like = not 'windows' in plat

def run():
    if unix_like: # NOT USABLE FOR UNIX LIKE SYSTEM
        return
    else:
        pass

if __name__ == '__main__':
    run()