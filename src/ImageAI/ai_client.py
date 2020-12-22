
import requests
import pprint as pp
import json
import sys
import os
import time
from requests.auth import HTTPBasicAuth
from datetime import datetime, date, time, timedelta
from bearer_auth import BearerAuth


class AIClient:
    def __init__(self):
        self.authority = os.environ["MEDIA_AUTHORITY"]
        self.client_id = os.environ["MEDIA_CLIENT_ID"]
        self.secret = os.environ["MEDIA_CLIENT_SECRET"]
        self.base_url = os.environ["MEDIA_API_URL"]
        print("api base_url: {}".format(self.base_url))
        print("authority: {}".format(self.authority))
        print("client_id: {}".format(self.client_id))
        self.token = None

    def get_without_ai(self):
        token = self.get_token()
        r = requests.get(
            '{}ai/MediaWithoutImageAISource'.format(self.base_url), auth=BearerAuth(token))
        r.raise_for_status()

        return r.json()

    def get_image(self, id):
        token = self.get_token()
        r = requests.get(
            '{}ai/image/{}'.format(self.base_url, id), auth=BearerAuth(token), stream=True)
        r.raise_for_status()

        return r.raw.read()

    def save_ai_data(self, data):
        token = self.get_token()
        r = requests.post(
            '{}ai/save'.format(self.base_url), auth=BearerAuth(token), json=data)
        r.raise_for_status()

    def get_token(self):
        print('get token')
        if (self.token is not None and self.token["expires_at"] > datetime.now()):
            return self.token["token"]

        data = {"grant_type": "client_credentials",
                "scope": "api.magic.imageai"}
        r = requests.post(
            '{}/connect/token'.format(self.authority), data=data, auth=HTTPBasicAuth(self.client_id, self.secret))
        r.raise_for_status()
        result = r.json()
        self.token = {"token": result["access_token"], "expires_at": datetime.now(
        ) + timedelta(seconds=result["expires_in"])}

        print("token will expire at: {}".format(self.token["expires_at"]))

        return self.token["token"]


if __name__ == "__main__":
    client = AIClient()

    medias = client.get_without_ai()
    pp.pprint(medias)
    #token = client.get_token()
    # print(token)
