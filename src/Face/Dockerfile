FROM hellozcb/face_recognition

RUN pip install --upgrade pip && \
  pip install imutils && \
  pip install opencv-python && \
  pip install --upgrade ptvsd && \
  pip install sklearn && \
  pip install Flask && \
  pip install isodate && \
  pip3 install tensorflow==1.15  && \
  pip3 install python-graphql-client && \
  #pip3 install --ignore-installed --upgrade https://github.com/lakshayg/tensorflow-build/releases/download/tf1.10.0-ubuntu18.04-py36-py27/tensorflow-1.10.0-cp36-cp36m-linux_x86_64.whl --user && \
  pip3 install pillow && \
  pip3 install matplotlib && \
  pip3 install h5py && \
  pip3 install keras==2.2.4 && \
  pip3 install https://github.com/OlafenwaMoses/ImageAI/releases/download/2.1.0/imageai-2.1.0-py3-none-any.whl

COPY . /app

RUN rm -f /app/models/shape_predictor_5_face_landmarks.dat.bz
RUN rm -f /app/models/shape_predictor_68_face_landmarks.dat.bz2

ADD https://github.com/davisking/dlib-models/raw/master/shape_predictor_5_face_landmarks.dat.bz2 /app/models/
ADD https://github.com/davisking/dlib-models/raw/master/shape_predictor_68_face_landmarks.dat.bz2 /app/models/

RUN bzip2 -d /app/models/shape_predictor_5_face_landmarks.dat.bz2
RUN bzip2 -d /app/models/shape_predictor_68_face_landmarks.dat.bz2

EXPOSE 5001

WORKDIR /app
CMD ["python3", "/app/app.py"]