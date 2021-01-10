import pprint
import os.path
import face_recognition
import re
from PIL import Image, ImageDraw
import cv2
import dlib
import pickle
from io import StringIO
import io
from sklearn import neighbors
import math
import base64
from imutils.face_utils import FaceAligner

class FaceEncoder:

    MODEL_PATH = None

    def __init__(self):

        self.MODEL_PATH = "./models/faces.knn"
        predictor = dlib.shape_predictor(
            "./models/shape_predictor_68_face_landmarks.dat")

        self.fa = FaceAligner(predictor)
        if os.path.isfile(self.MODEL_PATH):
            self.knn_model = self.load_model()

    def build_model(self, faces):
        # pprint.pprint(faces)

        X = []
        y = []

        for face in faces:
            if "encoding" in face:
                X.append(face["encoding"])
                y.append(str(face["personId"]))

        n_neighbors = int(round(math.sqrt(len(X))))
        knn_clf = neighbors.KNeighborsClassifier(
            n_neighbors=n_neighbors, algorithm="ball_tree", weights='distance')
        knn_clf.fit(X, y)

        with open(self.MODEL_PATH, 'wb') as f:
            pickle.dump(knn_clf, f)

        self.knn_model = knn_clf
        return len(X)

    def load_model(self):
        knn_clf = None
        with open(self.MODEL_PATH, 'rb') as f:
            knn_clf = pickle.load(f)
        return knn_clf

    def predict_face(self, face_encoding, distance_threshold):

        #knn_clf = self.load_model()

        faces_encodings = [face_encoding]
        # Use the KNN model to find the best matches for the test face
        closest_distances = self.knn_model.kneighbors(
            faces_encodings, n_neighbors=1)
        are_matches = [closest_distances[0][i][0]
                       <= distance_threshold for i in range(1)]

        # Predict classes and remove classifications that aren't within the threshold
        a = [(pred, loc) if rec else ("unknown", loc) for pred, loc, rec in zip(
            self.knn_model.predict(faces_encodings), {1, 2, 3, 4}, are_matches)]
        person = None

        pprint.pprint(a)
        if len(a) > 0 and a[0][0] != "unknown":
            if a[0][0] != "None":
                return a[0][0]
            return None

    def encode_face(self, file):

        image = face_recognition.load_image_file(file)

        face_bounding_boxes = face_recognition.face_locations(image)
        if len(face_bounding_boxes) > 0:
            gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

        faces = []

        for box in face_bounding_boxes:
            pprint.pprint(box)
            top, right, bottom, left = box
            if (right - left) < 50:
                continue
            rect = dlib.rectangle(left, top, right, bottom)
            face_aligned = self.fa.align(image, gray, rect)
            face_encodings = face_recognition.face_encodings(face_aligned)

            # When no faces found try not aligned
            if len(face_encodings) == 0:
                face_encodings = face_recognition.face_encodings(
                    image, known_face_locations=[box])
            # pprint.pprint(face_encoding)
            if len(face_encodings) > 0:
                face_encoding = face_encodings[0]
            else:
                return

            face_image = image[top:bottom, left:right]

            face = {"box": {"top": top, "right": right, "bottom": bottom, "left": left},
                    "encoding": face_encoding.tolist(),
                    "b64": base64.b64encode(pickle.dumps(face_encoding, protocol=2)).decode('utf-8')}
            faces.append(face)

        return faces
