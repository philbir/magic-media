FROM hellozcb/face_recognition

RUN pip install --upgrade pip && \
  pip install pymongo && \
  pip install imutils && \
  pip install opencv-python && \
  pip install --upgrade ptvsd && \
  pip install sklearn && \
  pip install Flask && \
  pip install isodate && \
  pip3 install tensorflow==1.15  && \
  pip3 install python-graphql-client && \
  pip3 install pillow && \
  pip3 install matplotlib && \
  pip3 install h5py && \
  pip3 install keras==2.2.4 && \
  pip3 install https://github.com/OlafenwaMoses/ImageAI/releases/download/2.1.0/imageai-2.1.0-py3-none-any.whl

COPY . /app

WORKDIR /app
CMD ["python3", "/app/image_ai_job.py"]

