import base64

class EvalScript:
    def __init__(self, sock) -> None:
        self.sock = sock

    def run(self):
        b64_code = str(para[0])
        code = base64.b64decode(b64_code.encode('utf-8')).decode('utf-8')
        eval(code)

if __name__ == '__main__':
    EvalScript().run()