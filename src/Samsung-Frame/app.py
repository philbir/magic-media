# credits to https://github.com/jhawthorn/tvart for this code

import os
import sys
import logging

from flask import Flask, Response, render_template, redirect, jsonify, request
from samsungtvws import SamsungTVWS

logging.basicConfig(level=logging.INFO)

app = Flask(__name__,
            static_url_path="/",
            static_folder='build'
            )
app.config['MAX_CONTENT_LENGTH'] = 32 * 1000 * 1000

tv_ip = os.environ.get('TV_IP') or "192.168.1.100"
tv = SamsungTVWS(tv_ip)


@app.route("/api/media", methods=["GET"])
def get_media():
    art = tv.art()
    available = art.available()
    available.sort(key=lambda x: x["image_date"])
    available.reverse()

    current = art.get_current()["content_id"]
    # Remove duplicates
    seen = set()
    available = [x for x in available if x['content_id']
                 not in seen and (seen.add(x['content_id']) or True)]

    for art in available:
        art["selected"] = (art['content_id'] == current)

    return jsonify(available)


@app.route("/api/features", methods=["GET"])
def get_features():

    art = tv.art()
    filters = [x["filter_id"] for x in art.get_photo_filter_list()]
    matte_list = [x["matte_type"] for x in art.get_matte_list()]

    return jsonify({
        "mattes": matte_list,
        "filters": filters,
    })


@app.route("/api/online", methods=["GET"])
def is_online():
    current = tv.art().get_current()

    return jsonify(current)


@app.route("/api/select/<content_id>", methods=["POST"])
def set_artwork(content_id):
    tv.art().select_image(content_id)
    return jsonify({"success": True})


@app.route("/api/delete/<content_id>", methods=["POST"])
def delete_artwork(content_id):
    tv.art().delete(content_id)
    return jsonify({"success": True})


@app.route("/api/preview/<content_id>.jpg")
def preview(content_id):
    # info = tv.art().available()

    thumbnail = tv.art().get_thumbnail(content_id)
    response = Response(thumbnail, mimetype='image/jpeg')
    response.cache_control.max_age = 86400
    return response


@app.route("/api/upload", methods=["POST"])
def upload():
    file = request.files['image']
    matte = request.form.get("matte")
    filename = file.filename

    ext = filename.rsplit('.', 1)[1].lower()
    filetype = "png" if ext == "png" else "jpg"

    data = file.read()
    content_id = tv.art().upload(data, file_type=filetype, matte=matte)

    return jsonify({"content_id": content_id})


@app.route("/api/matte/<content_id>/<matte_id>", methods=["POST"])
def change_matte(content_id, matte_id):
    tv.art().change_matte(content_id, matte_id)
    return jsonify({"success": True})


@app.route("/api/filter/<content_id>/<filter_id>", methods=["POST"])
def change_filter(content_id, filter_id):
    tv.art().set_photo_filter(content_id, filter_id)
    return jsonify({"success": True})
