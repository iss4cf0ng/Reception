import platform
import os

plat = platform.platform().lower()
unix_like = not 'windows' in plat
filename = 'malware.py'

def run():
    if unix_like: # Unix/Linux
        pass
    else: # Windows
        py_file = os.path.join([
            os.path.expanduser('~'),
            'AppData',
            'Roaming',
            'Microsoft',
            'Windows',
            'Start Menu',
            'Programs',
            'Startup',
            filename,
        ])

        with open(py_file, 'a') as f:
            pass

if __name__ == '__main__':
    run()