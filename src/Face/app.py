from flask import Flask
from flask import jsonify
from flask import request
from face_encoder import FaceEncoder
#from object_detection import ObjectDetector
import uuid
import numpy
import os
import io
import json

detector = None


def create_app():
    app = Flask(__name__)
    face = FaceEncoder()

    @app.route("/face/detect", methods=['POST'])
    def face_detect():
        file = request.files['file']
        data = file.read()

        faces = face.encode_face(file)

        return jsonify(faces)

    @app.route("/object/detect", methods=['POST'])
    def object_detect():
        file = request.files['file']
        #data = file.read()

        filename = str(uuid.uuid4())
        filename = os.path.join('./temp', filename + '.jpg')

        file.save(filename)
        objects = detector.detect(filename)

        return jsonify(objects)

    @app.route("/face/buildmodel", methods=['POST'])
    def build_model():

        faces = json.loads(request.data)
        faceCount = face.build_model(faces)
        return jsonify({"faceCount": faceCount})

    @app.route("/face/predict", methods=['POST'])
    def predict_person():

        data = json.loads(request.data)
        personId = face.predict_face(data["encoding"], data["distance"])
        return jsonify(personId)

    return app


if (__name__ == "__main__"):
    app = create_app()
    app.run(debug=True, host='0.0.0.0', port=5001, threaded=False)
