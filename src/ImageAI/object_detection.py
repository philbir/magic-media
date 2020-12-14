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

    def detect(self, filename, dimensions):
        
        print("Filename: {}".format(filename))    
        detections = self.detector.detectObjectsFromImage(
            input_image=filename, 
            input_type="file", 
            output_image_path="./tmp/out.jpg")

        items = []

        for detection in detections:
            obj = {}
            obj["type"] = "object"
            obj["name"] = detection["name"]
            obj["probability"] = detection["percentage_probability"]
            x1, y1, x2, y2 = detection["box_points"]
            obj["box"] = {}
            obj["box"]["left"] = int(x1)
            obj["box"]["top"] = int(y1)
            obj["box"]["bottom"] = dimensions["height"] - (dimensions["height"]- int(y2))
            obj["box"]["right"] = dimensions["width"] - (dimensions["width"]- int(x2))
            items.append(obj)

        predictions, probabilities = self.prediction.predictImage(
            filename, result_count=10)

        preds = []
        for idx, val in enumerate(predictions):
            obj = {}
            obj["type"] = "tag"
            obj["name"] = val
            obj["probability"] = probabilities[idx]
            items.append(obj)

        return items


if __name__ == "__main__":
    detector = ObjectDetector()
    detector.detect('./tmp/9055baaa-56eb-40a1-b68b-ff141ac22374.jpg')
    pp.pprint(object)