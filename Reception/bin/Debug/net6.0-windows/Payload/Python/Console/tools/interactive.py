from importlib.machinery import SourceFileLoader
from tools import general
from tools import converter
import platform
import os
import sys
import base64

class OutputAdjust:
    def __init__(self, args) -> None:
        self.stdout_list = []
        self.args = args # COMMAND ARGUMENT

    def write(self, output):
        if output == '\n':
            return
        
        if self.args.reception:
            output = converter.Converter(None, None).color_convert(output)
        
        self.stdout_list.append(output)

    def print_result(self):
        for line in self.stdout_list:
            print(line)

        self.stdout_list = []

    def flush(self):
        pass

class MyCmd():
    def __init__(self) -> None:
        self.args = None
        self.cd = ''
        self.module_dir = ''
        self.module_absPath = ''
        self.current_module = None # CURRENT MODULE

    def args_array(self, cmd:str) -> list:
        result = []
        for arg in cmd.split(' '):
            _arg = arg.strip()
            if _arg:
                result.append(_arg)

        return result
    
    def check_req(self, config:dict):
        bad_options = True # OK
        for key in config.keys():
            req = config[key]['req'].lower() == 'yes'
            if req:
                value = config[key]['value']
                if value == '':
                    general.print_err(f'\'{key}\' is required.')
                    bad_options = False # ERROR

        return bad_options

    def cmdloop(self):
        stdout_adjust = OutputAdjust(self.args)

        while True:

            sys.stdout = sys.__stdout__
            stdout_adjust.print_result()

            # CONVERT INPUT COMMAND ARGUMENT INTO LIST.
            prompt = 'Reception' if not self.cd else f'Reception({converter.Converter(self.current_module, self.module_absPath).color_convert(general.print_input(self.cd))})'
            if self.args.reception:
                command = input()
                print(f'{prompt}> {command}')
            else:
                command = input(f'{prompt}> ')

            sys.stdout = stdout_adjust

            args = self.args_array(command)
            if len(args) == 0:
                continue
            
            main_cmd = args[0]
            if main_cmd == 'use':
                if len(args) == 1:
                    continue

                path = args[1]
                py_path = f'{os.path.join(self.module_dir, path)}.py'
                if not os.path.exists(py_path):
                    general.print_err(f'Module : "{path}" not exists.')
                    continue

                self.cd = path
                self.module_absPath = py_path
                name = args[1].split('/')[-1]
                module = SourceFileLoader(name, py_path).load_module()
                self.current_module = module.Module()
                self.current_module.module_path = path
                if module:
                    print(f'=> {self.cd}')

            elif main_cmd == 'set':
                config = dict(self.current_module.config)
                key = args[1]
                value = args[2]
                _config = general.config_setValue(config, key, value)
                if not _config:
                    general.print_err(f'key \'{key}\' not found')
                    continue

                self.current_module.config[_config[0]]['value'] = value
                print(f'{key} => {value}')

            elif main_cmd == 'show':
                if not self.current_module:
                    general.print_err('No module in use...')

                if hasattr(self.current_module, 'config'):
                    if self.current_module == None:
                        general.print_err('No module...')
                        continue

                    if len(args) == 2:
                        kv = general.get_value(self.current_module.config, args[1])

                        if kv:
                            general.show_available(self.current_module.config, kv[0])
                            continue

                    general.show_options(self.current_module.config, self.cd)
                else:
                    general.print_info('No config found, it can be run directly.')
            elif main_cmd == 'run':
                convert = converter.Converter(self.current_module, self.module_absPath)

                if hasattr(self.current_module, 'config'):
                    if not self.check_req(self.current_module.config):
                        continue

                    config = self.current_module.config
                    self.current_module.config = convert.configStr2Int(config)

                if self.args.reception:
                    payload = convert.payload()
                    split_str = '[RECEPTION]'
                    b64_payload = base64.b64encode(payload.encode('utf-8')).decode('utf-8')
                    print(f'{split_str}{b64_payload}{split_str}')
                else:
                    self.current_module.run()

            elif main_cmd == 'help':
                if self.current_module:
                    if hasattr(self.current_module, 'help'):
                        print(self.current_module.help)
                else:
                    pass
            elif main_cmd == 'exit':
                if self.current_module:
                    self.cd = ''
                    self.current_module = None
                else:
                    break

    def run(self):
        self.cmdloop()