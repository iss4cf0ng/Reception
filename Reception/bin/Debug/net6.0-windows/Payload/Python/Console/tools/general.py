import platform
import os

# MODULE CONFIG
REMOTE_CONFIG = {
    'IP' : {
        'value' : '127.0.0.1',
        'req' : 'yes',
        'help' : 'Remote IP address, use \',\' for multi ip addresses',
    },
    'Port' : {
        'value' : '',
        'req' : 'yes',
        'help' : 'Remote port.',
    },
    'Timeout' : {
        'value' : '10',
        'req' : 'yes',
        'help' : 'Send packet timeout(seconds).'
    }
}

LOCAL_CONFIG = {
    
}

# INITIALIZATION
class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

if 'windows' in platform.platform().lower():
    os.system('color')

# COMMAND
def get_value(config:dict, key:str) -> tuple:
    key_list = [str(c).lower() for c in config.keys()]
    if key in key_list:
        index = key_list.index(key)
        config_key = list(config.keys())[index]
        config_value = config[config_key]['value']
        return tuple([config_key, config_value])
    else:
        return None
    
def config_new_item(default, req, desc):
    config = {
        'value' : default,
        'req' : req,
        'help' : desc,
    }

    return config

def show_columnsHeader(ch, max_len):
    pass

def show_items(data):
    pass

def show_options(config_dict:dict, module_path:str) -> None:
    ch = ['Name', 'Current Setting', 'Required', 'Description'] # COLUMN HEADERS
    max_len = [len(header) for header in ch] # COLUMN HEADER WIDTH

    keys = [key for key in list(config_dict.keys())]
    values = [d['value'] for d in config_dict.values()]
    reqs = [d['req'] for d in config_dict.values()]
    helps = [d['help'] for d in config_dict.values()]

    keys_maxLen = max([len(str(i)) for i in keys])
    values_maxLen = max([len(str(i)) for i in values])
    reqs_maxLen = max([len(str(i)) for i in reqs])
    helps_maxLen = max([len(str(i)) for i in helps])

    _len = [keys_maxLen, values_maxLen, reqs_maxLen, helps_maxLen]

    for i in range(0, len(_len)):
        max_len[i] = _len[i] if max_len[i] < _len[i] else max_len[i]

    # PRINT COLUMN HEADERS
    print(f'Module options ({module_path}) :\n')
    print(f'%-{max_len[0]}s\t%-{max_len[1]}s\t\t%-{max_len[2]}s\t%-{max_len[3]}s' % (ch[0], ch[1], ch[2], ch[3]))
    print(f'%-{max_len[0]}s\t%-{max_len[1]}s\t\t%-{max_len[2]}s\t%-{max_len[3]}s' % (len(ch[0])*'-', len(ch[1])*'-', len(ch[2])*'-', len(ch[3])*'-'))

    for i in range(0, len(keys)):
        print(f'%-{max_len[0]}s\t%-{max_len[1]}s\t\t%-{max_len[2]}s\t%-{max_len[3]}s' % (
            keys[i],
            values[i],
            reqs[i],
            helps[i]
        ))

    print('\n')

def show_available(config:dict, key:str) -> None:
    if 'available' in dict(config[key]).keys():
        key_info = dict(config[key]['available'])
        for item in key_info.keys():
            print('\t%-10s=>\t%s' % (item, key_info[item]))

def config_setValue(config:dict, key, value) -> tuple:
    kv_tuple = get_value(config, key)
    if kv_tuple:
        _key = kv_tuple[0]
        config[_key]['value'] = value
    
    return kv_tuple

def print_input(cd):
    return f'{bcolors.FAIL}{cd}{bcolors.ENDC}'

def print_info(msg):
    print(f'{bcolors.OKBLUE}[*]{bcolors.ENDC} {msg}')

def print_succ(msg):
    print(f'{bcolors.OKGREEN}[+]{bcolors.ENDC} {msg}')

def print_warn(msg):
    print(f'{bcolors.WARNING}[!]{bcolors.ENDC} {msg}')

def print_err(msg):
    print(f'{bcolors.FAIL}[-]{bcolors.ENDC} {msg}')