import subprocess
import platform

class PCControl:
    def __init__(self, sock) -> None:
        self.sock = sock
        self.command = {
            'shutdown' : {
                'linux' : 'shutdown now +[SEC]',
                'windows' : 'shutdown /s /t [SEC]',
            },
            'logout' : {
                'linux' : 'gnome-session-quit --logout --force --delay [SEC]',
                'windows' : 'shutdown /l /t [SEC]',
            },
            'restart' : {
                'linux' : 'shutdown -r +[SEC]',
                'windows' : 'shutdown /r /t [SEC]',
            }
        }
        self.platform = 'windows' if 'windows' in platform.platform().lower() else 'linux'

    def cmd(self, _type, sec):
        try:
            cmd = self.command[_type][self.platform].replace('[SEC]', sec)
            subprocess.check_output([cmd])
            return '1|'
        except Exception as e:
            return '0|'

    def run(self):
        cmd = para[0] # COMMAND
        sec = para[1] # TIME (SECOND)
        if cmd == 's': # SHUTDOWN
            self.cmd('shutdown', sec)
        elif cmd == 'l': # LOGOUT
            self.cmd('logout', sec)
        elif cmd == 'r': # RESTART
            self.cmd('restart', sec)