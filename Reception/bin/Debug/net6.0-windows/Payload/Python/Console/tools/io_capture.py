import sys

class Stdout2List:
    def __init__(self, redirect_event) -> None:
        self.stdout_list = []
        self.redirect_event = redirect_event

    def write(self, msg):
        self.stdout_list.append(msg)

    def flush(self):
        pass

def _event():
    pass

stdout_to_list = Stdout2List(redirect_event=_event)

sys.stdout = stdout_to_list

print('hello')
print('world')

sys.stdout = sys.__stdout__

print(stdout_to_list.stdout_list)