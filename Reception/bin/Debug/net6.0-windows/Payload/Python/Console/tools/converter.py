'''
CONTEXT CONVERTER
'''

import json
import re

class Converter:
    def __init__(self, module, path) -> None:
        self.config = None # COPY FROM PAYLOAD
        self.module = module
        self.module_path = path # ABSOLUTE PATH
        self.config_dict = ''
        self.config_general_existed = False

        self.remove_dict = False
        self.dict_bracket = [0, 0]

        self.color_map = {
            '\033[95m': '<color=Purple>',
            '\033[94m': '<color=Blue>',
            '\033[96m': '<color=Cyan>',
            '\033[92m': '<color=Green>',
            '\033[93m': '<color=Yellow>',
            '\033[91m': '<color=Red>',
            '\033[0m': '</color>',
        }

    def configStr2Int(self, config:dict) -> dict:
        for key in config.keys():
            value = str(config[key]['value'])
            if value.isdigit():
                config[key]['value'] = int(value)
        
        return config

    def config2dictStr(self) -> str:
        module = self.module
        if hasattr(module, 'config'):
            config = dict(module.config)
            config = self.configStr2Int(config)

            dict_str = json.dumps(config, indent=0).replace('\n', '')
            return dict_str
        else:
            return ''
    
    def code_doctor(self, code_line:str):
        if 'import' in code_line or 'from' in code_line:
            if 'tools' in code_line and 'general' in code_line:
                return
        
        if hasattr(self.module, 'config'):
            if 'self.config' in code_line:
                if 'general' in code_line:
                    self.config_general_existed = True
                    return code_line.replace('general.REMOTE_CONFIG', self.config_dict).replace('general.LOCAL_CONFIG', self.config_dict)

                if '{' in code_line: # and self.config_general_existed
                    with open(self.module_path, 'r') as f:
                        payload = f.read()
                    self.remove_dict = True
                    self.dict_bracket[0] += 1
                    return f'        self.config = {self.config_dict}'
                
            if self.remove_dict:
                if '{' in code_line:
                    self.dict_bracket[0] += 1
                elif '}' in code_line:
                    self.dict_bracket[1] += 1
                    if self.dict_bracket[0] == self.dict_bracket[1]:
                        self.remove_dict = False
                        self.dict_bracket = [0, 0]
                return

            #if ('{' in code_line) or (('\'' in code_line) and (':' in code_line) and (',' in code_line) and ('for' not in code_line)) or '}' in code_line:
            #    return

        return code_line

    def payload(self):
        dict_str = self.config2dictStr()
        self.config_dict = dict_str

        with open(self.module_path, 'r') as f:
            payload = f.read()

        new_payload = ''
        for line in payload.split('\n'):
            new_line = self.code_doctor(line)
            if new_line:
                new_payload += f'{new_line}\n'

        return new_payload
    
    def color_convert(self, ansi_line:str) -> str:
        result = ansi_line
        for key in self.color_map.keys():
            #result = result.replace(key, self.color_map[key])
            result = result.replace(key, '')

        return result