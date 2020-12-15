import os
import requests
from requests.auth import HTTPBasicAuth

class TokenResolver:

    def __init__(self):
        print('Init Token resolver')
        self.authority = os.environ["IDENTITY_AUTHORITY"]
        self.client_id = os.environ["IDENTITY_CLIENT_ID"]
        self.client_secret = os.environ["IDENTITY_CLIENT_SECRET"]


    def getToken(self):

        auth = HTTPBasicAuth(self.client_id, self.client_secret)
        payload = {'grant_type':'client_credentials'}
        r = requests.post(self.authority + "/connect/token", data = payload)

        print(r.json())

