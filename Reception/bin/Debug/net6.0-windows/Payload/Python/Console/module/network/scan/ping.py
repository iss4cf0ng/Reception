try:
    from tools import general
    from scapy.all import sr, IP, UDP, TCP
except ModuleNotFoundError as e:
    print(f'[-] {str(e)}')

class Module:
    def __init__(self) -> None:
        self.type = 'REMOTE'
        self.module_path = ''
        self.config = general.REMOTE_CONFIG

        self.config['Protocol'] = {
            'value' : 'TCP',
            'req' : 'yes',
            'help' : 'Protocol use for ping. (Case insensitive)',
            'available' : {
                'ARP' : 'Adress Resolution Protocol',
                'ICMP' : 'Internet Control Message Protocol',
                'TCP' : 'Transmission Control Protocol',
                'UDP' : 'User Datagram Protocol',
            }
        }

    def tcp_ping(self):
        tgt_ip = self.config['IP']['value']
        tgt_port = self.config['Port']['value']
        for ip in tgt_ip.split(','):
            ip_pkt = IP(dst=ip)
            tcp_pkt = TCP(dport=tgt_port, flags=0x012)
            pkt = ip_pkt/tcp_pkt
            ans, un_ans = sr(pkt, timeout=2)
            for s, r in ans:
                print(r.sprintf('[+] %IP.src% is alive'))
            for s in un_ans:
                print('Target is not alive.')

    def udp_ping(self):
        tgt_ip = self.config['IP']['value']
        tgt_port = self.config['Port']['value']
        pkt = IP(dst=tgt_ip)/UDP(dport=tgt_port)
        ans, un_ans = sr(pkt, timeout=2)
        ans.summary(lambda s, r : r.sprintf(f'{IP.src} is alive.'))

    def icmp_ping(self):
        pass

    def arp_ping(self):
        pass

    def run(self):
        protocol = self.config['Protocol']['value'].lower()
        if protocol == 'tcp':
            self.tcp_ping()
        elif protocol == 'udp':
            self.udp_ping()