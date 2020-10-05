from imageai.Detection import ObjectDetection
from imageai.Prediction import ImagePrediction

import os
import pprint as pp
import json
import PIL

class ObjectDetector:

    def __init__(self):
        self.detector = ObjectDetection()
        self.detector.setModelTypeAsRetinaNet()
        self.detector.setModelPath(
            "./models/resnet50_coco_best_v2.0.1.h5")
        self.detector.loadModel()

        self.prediction = ImagePrediction()
        self.prediction.setModelTypeAsResNet()
        self.prediction.setModelPath(
            "./models/resnet50_weights_tf_dim_ordering_tf_kernels.h5")
        self.prediction.loadModel()

    def detect(self, file):

        self.detector.loadModel()
        detections = self.detector.detectObjectsFromImage(input_image=file, output_image_path="DDDD.jpg")

        print( len(detections))
        objects = []
        for detection in detections:
            obj = {}
            obj["type"] = "object"
            obj["model"] = "resnet50_coco_best_v2.0.1.h5"
            obj["name"] = detection["name"]
            obj["probability"] = detection["percentage_probability"]
            x1, y1, x2, y2 = detection["box_points"]
            obj["box"] = {}
            obj["box"]["x1"] = int(x1)
            obj["box"]["y1"] = int(y1)
            obj["box"]["x2"] = int(x2)
            obj["box"]["y2"] = int(y2)
            objects.append(obj)
            print(detection["name"])

        return object


if __name__ == "__main__":
    detector = ObjectDetector()

    detector.detect('./temp/479c2585-009e-4b24-adde-417515cb8f27.jpg')
    pp.pprint(object)