<template>
  <v-card height="100%" :loading="loading">
    <div class="image-wrapper">
      <img :src="`/api/media/webimage/${mediaId}`" />
    </div>
    <v-toolbar v-if="!loading" color="gray" dense dark>
      <h4 class="d-none d-lg-inline">{{ media ? media.filename : "" }}</h4>
      <v-tabs v-model="tab" dark class="ml-lg-8 ml-sm-0">
        <v-tab href="#attributes"> Attributes </v-tab>
        <v-tab href="#location" v-if="hasLocation"> Location </v-tab>
        <v-tab href="#faces" v-if="media.faces.length > 0"> Faces </v-tab>
        <v-tab href="#ai"> AI </v-tab>
        <v-tab href="#files"> Files </v-tab>
        <v-tab href="#raw"> Raw </v-tab>
      </v-tabs>
      <v-spacer></v-spacer>

      <v-icon class="mr-4" color="white" @click="back"> mdi-arrow-left </v-icon>
    </v-toolbar>

    <v-card-text class="card-content" v-if="!loading">
      <v-row>
        <v-col sm="12">
          <v-tabs-items v-model="tab" style="background-color: transparent">
            <v-tab-item value="attributes">
              <v-sheet
                width="600"
                elevation="10"
                rounded="lg"
                class="ma-2 pa-4 card-details-content"
              >
                <v-row v-for="(item, i) in attributes" :key="i">
                  <v-col cols="12" sm="2">{{ item.label }}</v-col>
                  <v-col cols="12" sm="10" class="font-weight-bold">{{
                    item.value
                  }}</v-col>
                </v-row>
              </v-sheet>
            </v-tab-item>
            <v-tab-item value="location" v-if="hasLocation">
              <v-sheet
                elevation="10"
                rounded="lg"
                class="ma-2 pa-4 card-details-content"
              >
                <v-row>
                  <v-col lg="5" sm="0" class="d-none d-lg-block">
                    <v-row v-for="(item, i) in location" :key="i">
                      <v-col cols="12" sm="2">{{ item.label }}</v-col>
                      <v-col cols="12" sm="10" class="font-weight-bold">{{
                        item.value
                      }}</v-col>
                    </v-row></v-col
                  >
                  <v-col
                    lg="7"
                    sm="12"
                    style="background-color: #fff; z-index: 1000"
                    class="ma-0 pa-0"
                  >
                    <GmapMap
                      :center="map.center"
                      :zoom="12"
                      :options="map.options"
                      ref="mapRef"
                      map-type-id="roadmap"
                      style="width: 100%; height: 580px"
                    >
                      <GmapMarker
                        v-if="map.center"
                        :position="map.center" /></GmapMap
                  ></v-col>
                </v-row>
              </v-sheet>
            </v-tab-item>

            <v-tab-item value="faces" style="background-color: transparent">
              <div class="face-conainer">
                <v-row
                  v-for="face in media.faces"
                  :key="face.id"
                  class="face-row"
                  @click="editFace(face)"
                  :style="{ 'border-left-color': faceColor(face) }"
                >
                  <v-col sm="6">
                    <img
                      class="face-image"
                      :src="`/api/face/${face.id}/thumbnail/${face.thumbnail.id}`"
                    />
                  </v-col>
                  <v-col sm="6">
                    <v-row>
                      <v-col sm="12" class="pa-0">
                        <h1>
                          {{ faceTitle(face) }}
                        </h1>
                        <h4 class="mt-2">{{ age(face) }}</h4>
                      </v-col>
                      <v-col sm="12" class="ma-0 pa-0">
                        <v-row
                          v-for="(item, i) in getFaceProperties(face)"
                          :key="i"
                        >
                          <v-col class="font-weight-bold">{{
                            item.value
                          }}</v-col>
                        </v-row>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col class="pa-0"> </v-col>
                    </v-row>
                  </v-col>
                </v-row>
              </div>
            </v-tab-item>
            <v-tab-item value="ai" v-if="media.ai">
              <v-sheet
                v-if="media.ai.caption"
                elevation="10"
                rounded="lg"
                class="ma-2 pa-4 card-details-content"
              >
                <h3>
                  {{ media.ai.caption.text }}
                  <small>
                    ({{
                      Math.round(media.ai.caption.confidence * 100) / 100
                    }}%)</small
                  >
                </h3>
              </v-sheet>

              <v-row>
                <v-col lg="4">
                  <v-sheet
                    v-if="media.ai.tags.length > 0"
                    width="300"
                    elevation="10"
                    rounded="lg"
                    class="ma-2 pa-4 card-details-content"
                  >
                    <h3>Tags</h3>
                    <v-row
                      class="ma-0 pa-0"
                      v-for="(tag, i) in media.ai.tags"
                      :key="i"
                    >
                      <v-col cols="12" sm="8">{{ tag.name }}</v-col>
                      <v-col
                        cols="12"
                        sm="4"
                        class="font-weight-bold"
                        style="position: relative"
                        >{{ Math.round(tag.confidence * 100) / 100 }}
                        <div
                          :title="tag.source"
                          :class="`dot-ai-source ${tag.source.toLowerCase()}`"
                        ></div>
                      </v-col>
                    </v-row>
                  </v-sheet>
                </v-col>
                <v-col lg="4">
                  <v-sheet
                    v-if="media.ai.objects.length > 0"
                    width="300"
                    elevation="10"
                    rounded="lg"
                    class="ma-2 pa-4 card-details-content"
                  >
                    <h3>Objects</h3>
                    <v-row
                      class="ma-0 pa-0"
                      v-for="(obj, i) in media.ai.objects"
                      :key="i"
                    >
                      <v-col cols="12" sm="8">{{ obj.name }}</v-col>
                      <v-col
                        cols="12"
                        sm="4"
                        class="font-weight-bold"
                        style="position: relative"
                        >{{ Math.round(obj.confidence * 100) / 100 }}
                        <div
                          :title="obj.source"
                          :class="`dot-ai-source ${obj.source.toLowerCase()}`"
                        ></div
                      ></v-col>
                    </v-row>
                  </v-sheet>
                </v-col>
                <v-col lg="4" v-if="media.ai.colors">
                  <v-sheet
                    width="300"
                    elevation="10"
                    rounded="lg"
                    class="ma-2 pa-4 card-details-content"
                  >
                    <h3>Colors</h3>
                    <v-row class="ma-0 pa-0">
                      <v-col cols="12" sm="6">Accent</v-col>
                      <v-col cols="12" sm="6" class="font-weight-bold">
                        <div
                          class="color-box"
                          :style="{
                            'background-color': getColorString(
                              media.ai.colors.accent
                            ),
                          }"
                        >
                          &nbsp;
                        </div>
                      </v-col>
                    </v-row>
                    <v-row class="ma-0 pa-0">
                      <v-col cols="12" sm="6">Foreground</v-col>
                      <v-col cols="12" sm="6" class="font-weight-bold">
                        <div
                          class="color-box"
                          :style="{
                            'background-color': getColorString(
                              media.ai.colors.dominantForeground
                            ),
                          }"
                        >
                          &nbsp;
                        </div>
                      </v-col>
                    </v-row>
                    <v-row class="ma-0 pa-0">
                      <v-col cols="12" sm="6">Background</v-col>
                      <v-col cols="12" sm="6" class="font-weight-bold">
                        <div
                          class="color-box"
                          :style="{
                            'background-color': getColorString(
                              media.ai.colors.dominantBackground
                            ),
                          }"
                        >
                          &nbsp;
                        </div>
                      </v-col>
                    </v-row>
                  </v-sheet>
                </v-col>
              </v-row>
            </v-tab-item>
            <v-tab-item value="files">
              <v-row
                v-for="file in media.files"
                :key="file.filename"
                class="file-row"
              >
                <v-col>
                  <h3>{{ file.filename }}</h3>
                  <h4>{{ file.location }}</h4>
                  <h4>{{ file.type }}</h4>
                  <h4>{{ formatSize(file.size) }}</h4>
                </v-col>
              </v-row>
            </v-tab-item>
            <v-tab-item value="raw">
              <v-sheet
                elevation="10"
                rounded="lg"
                class="ma-2 pa-4 card-details-content"
              >
                <div class="json-wrapper">
                  <vue-json-pretty :data="media"> </vue-json-pretty>
                </div>
              </v-sheet>
            </v-tab-item>
          </v-tabs-items>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import "vue-json-pretty/lib/styles.css";
import VueJsonPretty from "vue-json-pretty";
import { getFaceColor } from "../../services/faceColor";
import { getInfo } from "../../services/mediaService";

export default {
  components: { VueJsonPretty },
  props: {
    mediaId: String,
  },
  data() {
    return {
      tab: "attributes",
      media: null,
    };
  },
  created() {
    this.loadInfo(this.mediaId);
  },
  watch: {
    mediaId: function (newValue) {
      this.loadInfo(newValue);
    },
  },
  computed: {
    loading: function () {
      return this.media == null;
    },
    map: function () {
      let center = null;

      if (this.hasLocation) {
        const { point } = this.media.geoLocation;

        center = {
          lat: point.coordinates[1],
          lng: point.coordinates[0],
        };
      }

      return {
        center: center,
        options: {
          zoomControl: false,
          mapTypeControl: true,
          scaleControl: true,
          streetViewControl: true,
          rotateControl: false,
          fullscreenControl: false,
          disableDefaultUi: false,
        },
        marker: null,
      };
    },
    hasLocation: function () {
      return this.media.geoLocation != null;
    },
    location: function () {
      const geoProps = [];

      if (this.hasLocation) {
        const loc = this.media.geoLocation;

        if (this.media.geoLocation.address != null) {
          geoProps.push(... [
            {
              label: "Address",
              value: loc.address.name,
            },
            {
              label: "Street",
              value: loc.address.address,
            },
            {
              label: "City",
              value: loc.address.city,
            },
            {
              label: "Country",
              value: loc.address.country,
            },
            {
              label: "District 1",
              value: loc.address.distric1,
            },
            {
              label: "District 2",
              value: loc.address.distric2,
            },
          ]);
        }

        geoProps.push(... [
          {
            label: "Coordinates",
            value: `${this.media.geoLocation.point.coordinates[0]}, ${this.media.geoLocation.point.coordinates[1]}`,
          },
          {
            label: "Altitude",
            value: loc.altitude,
          },
        ]);
      }

      return geoProps;
    },
    attributes: function () {
      var data = [
        {
          label: "Date taken",
          value: this.$options.filters.dateformat(
            this.media.dateTaken,
            "DATETIME_MED"
          ),
        },
        {
          label: "Folder",
          value: this.media.folder,
        },
        {
          label: "Size",
          value: this.formatSize(this.media.size),
        },
        {
          label: "Dimension",
          value: `${this.media.dimension.width} x ${this.media.dimension.height}`,
        },
        {
          label: "Favorite",
          value: this.isFavorite ? "yes" : "no",
        },
      ];

      if (this.media.camera) {
        data.push({
          label: "Camera",
          value: this.media.camera.make + " " + this.media.camera.model,
        });
      }

      if (this.media.videoInfo) {
        data.push({
          label: "Duration",
          value: this.media.videoInfo.duration,
        });
        data.push({
          label: "Format",
          value: this.media.videoInfo.format,
        });
        data.push({
          label: "Framerate",
          value: this.media.videoInfo.frameRate,
        });
      }

      data.push({
        label: "Source",
        value: this.media.source.identifier,
      });
      data.push({
        label: "Imported at",
        value: this.$options.filters.dateformat(
          this.media.source.importedAt,
          "DATETIME_MED"
        ),
      });

      return data;
    },
  },
  methods: {
    async loadInfo(mediaId) {
      if (mediaId) {
        var res = await getInfo(mediaId);
        this.media = res.data.mediaById;
        console.log(this.media);
      } else {
        this.media = null;
      }
    },
    getColorString: function (col) {
      if (col.length === 6) return "#" + col;
      return col;
    },
    getFaceProperties: function (face) {
      return [
        {
          label: "State / Type",
          value: `${face.state} / ${face.recognitionType}`,
        },
      ];
    },
    back: function () {
      this.$emit("back");
    },
    faceTitle: function (face) {
      let name = "Unknown";
      if (face.person) {
        name = face.person.name;
      }
      return name;
    },
    age: function (face) {
      if (!face.age) {
        return "";
      } else if (face.age < 12) {
        return face.age + " months";
      } else {
        return Math.floor(face.age / 12) + " years";
      }
    },
    faceColor: function (face) {
      return getFaceColor(face);
    },
    editFace: function (face) {
      this.$store.dispatch("face/openEdit", face);
    },
    formatSize: function (size) {
      return (size / 1024.0 / 1024.0).toPrecision(2) + " MB";
    },
  },
};
</script>

<style scoped>
.json-wrapper {
  height: 80vh;
  overflow: auto;
}

.image-wrapper {
  position: absolute;
  left: 0px;
  top: 0px;
  height: 100vh;
  width: 100%;
}

.image-wrapper img {
  object-fit: cover;
  height: 100%;
  width: 100%;
}

.card-transparent {
  background-color: transparent !important;
}

.card-details-content {
  background-color: rgb(36, 36, 36);
  opacity: 0.85;
  color: #fff !important;
  max-height: 84vh;
  overflow-x: auto;
}

.face-image {
  width: 200px;
  height: 200px;
  border-radius: 100%;
}

.face-row {
  background-color: rgb(36, 36, 36);
  margin: 8px;
  border-radius: 20px;
  width: 460px;
  float: left;
  opacity: 0.9;
  color: #fff;
}

.face-conainer {
  height: 90vh;
  overflow-x: scroll;
}

.file-row {
  background-color: rgb(36, 36, 36);
  margin: 8px;
  border-radius: 20px;
  float: left;
  opacity: 0.9;
  color: #fff;
}

.color-box {
  height: 24px;
  width: 24px;
  border: 1px solid #fff;
}

.dot-ai-source {
  position: absolute;
  height: 10px;
  width: 10px;
  border-radius: 100%;
  left: 56px;
  top: 18px;
}

.dot-ai-source.image_a_i {
  background-color: #009261;
  box-shadow: 0 0 6px 2px #34ffbb;
}

.dot-ai-source.azure_c_v {
  background-color: #d900b8;
  box-shadow: 0 0 6px 2px #f740db;
}
</style>>
