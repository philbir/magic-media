FROM python:3.5

RUN pip install --upgrade pip && \
  pip install numpy && \
  pip install scipy && \
  pip install opencv-python && \
  pip install --upgrade ptvsd && \
  pip3 install tensorflow==1.15  && \
  pip3 install pillow && \
  pip3 install matplotlib && \
  pip3 install h5py && \
  pip3 install keras==2.2.4 && \
  pip3 install https://github.com/OlafenwaMoses/ImageAI/releases/download/2.1.0/imageai-2.1.0-py3-none-any.whl

RUN apt update && apt install libgl1-mesa-glx -y
RUN pip install requests