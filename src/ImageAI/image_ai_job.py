from object_detection import ObjectDetector
from ai_client import AIClient
import requests
import pprint as pp
from PIL import Image
import io
import os
import json
import sys
import time

class ImageAIJob:

    def __init__(self):
        self.detector = ObjectDetector()
        self.client = AIClient()

    def run(self):

        medias = self.client.get_without_ai()

        print("Found {} images without ImageAI".format(len(medias)))

        for media in medias:

            id = media["id"]
            result = {}
            result["mediaId"] = id
            print("detect items for media: {}".format(id))
            filename = "./tmp/{}.jpg".format(id)

            try:
                img = self.client.get_image(id)
                with open(filename, 'wb') as f:
                    f.write(img)

                items = self.detector.detect(filename, media["dimension"])

                result["items"] = items
                result["success"] = True

                print("Found {} items in media: {}".format(
                    len(result["items"]), id))

                os.remove(filename)

            except Exception as e:
                result["success"] = False
                result["error"] = str(e)
                print(result["error"])

            self.client.save_ai_data(result)

        return len(medias)


if __name__ == "__main__":
    job = ImageAIJob()

    error_count = 0
    while True:
        try:
            count = job.run()
            if count == 0:
                print('No media without ImageAI found, taking a 10 min nap...')
                time.sleep(600)
        except Exception as e:
            print(str(e))
            time.sleep(5)
            error_count += 1

        if error_count > 10:
            sys.exit(1)