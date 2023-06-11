# syntax=docker/dockerfile:1

FROM python:3-slim
WORKDIR /app
COPY requirements.txt requirements.txt
RUN pip3 install -r requirements.txt

COPY app.py app.py

EXPOSE 8085
CMD [ "waitress-serve", "--port=8085", "app:app" ]
