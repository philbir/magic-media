<template>
  <v-card height="100%" :loading="loading">
    <div class="image-wrapper">
      <img :src="`/api/media/webimage/${mediaId}`" />
    </div>
    <v-toolbar v-if="!loading" color="gray" dense dark>
      <h4>{{ media ? media.filename : "" }}</h4>
      <v-tabs v-model="tab" dark class="ml-8">
        <v-tab href="#attributes"> Attributes </v-tab>
        <v-tab href="#location" v-if="hasLocation"> Location </v-tab>
        <v-tab href="#faces" v-if="media.faces.length > 0"> Faces </v-tab>
        <v-tab href="#objects"> Objects </v-tab>
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
                  <v-col sm="5">
                    <v-row v-for="(item, i) in location" :key="i">
                      <v-col cols="12" sm="2">{{ item.label }}</v-col>
                      <v-col cols="12" sm="10" class="font-weight-bold">{{
                        item.value
                      }}</v-col>
                    </v-row></v-col
                  >
                  <v-col
                    sm="7"
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
                      :src="'/api/media/thumbnail/' + face.thumbnail.id"
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
            <v-tab-item value="objects"> Objects </v-tab-item>
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
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";
import { getFaceColor } from "../../services/faceColor";
import { getInfo } from "../../services/mediaService";

export default {
  components: {
    VueJsonPretty,
  },
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
      if (this.hasLocation) {
        const loc = this.media.geoLocation;
        return [
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
          {
            label: "Coordinates",
            value: `${this.media.geoLocation.point.coordinates[0]}, ${this.media.geoLocation.point.coordinates[1]}`,
          },
          {
            label: "Altitude",
            value: loc.altitude,
          },
        ];
      }

      return [];
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
      } else {
        this.media = null;
      }
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
</style>>
