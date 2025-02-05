try:
    from tools import general
    import scapy.all
except ModuleNotFoundError as e:
    print(f'[-] {str(e)}')

class Module:
    def __init__(self) -> None:
        self.type = 'REMOTE'
        self.module_path = ''
        self.config = general.REMOTE_CONFIG

        self.config['Mode'] = {
            'value' : 'sS',
            'req' : 'yes',
            'help' : 'Port scanner.',
            'available' : {
                'ACK' : '',
                'SYN' : '',
                'TCP' : '',
            }
        }

    def scan_ack(self):
        pass

    def scan_syn(self):
        pass

    def scan_tcp(self):
        pass
        
    def run(self):
        pass