<template>
  <v-card height="100%">
    <div class="image-wrapper">
      <img :src="`/api/media/webimage/${media.id}`" />
    </div>
    <v-toolbar color="gray" dense dark>
      <h4>{{ media.filename }}</h4>
      <v-tabs v-model="tab" dark class="ml-8">
        <v-tab href="#attributes"> Attributes </v-tab>
        <v-tab href="#location"> Location </v-tab>
        <v-tab href="#faces" v-if="media.faces.length > 0"> Faces </v-tab>
        <v-tab href="#objects"> Objects </v-tab>
        <v-tab href="#raw"> Raw </v-tab>
      </v-tabs>
      <v-spacer></v-spacer>

      <v-icon class="mr-4" color="white" @click="back"> mdi-arrow-left </v-icon>
    </v-toolbar>

    <v-card-text class="card-content">
      <v-row>
        <v-col sm="12">
          <v-tabs-items v-model="tab">
            <v-tab-item value="attributes" class="card-details-content">
              <v-card elevation="2">
                <v-card-text>
                  <v-row v-for="(item, i) in attributes" :key="i">
                    <v-col cols="12" sm="2">{{ item.label }}</v-col>
                    <v-col cols="12" sm="10" class="font-weight-bold">{{
                      item.value
                    }}</v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item value="location">
              <v-card elevation="2">
                <v-card-text>
                  <v-row>
                    <v-col sm="5">
                      <v-row v-for="(item, i) in location" :key="i">
                        <v-col cols="12" sm="2">{{ item.label }}</v-col>
                        <v-col cols="12" sm="10" class="font-weight-bold">{{
                          item.value
                        }}</v-col>
                      </v-row></v-col
                    >
                    <v-col sm="7">
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
                </v-card-text>
              </v-card>
            </v-tab-item>

            <v-tab-item value="faces">
              <v-card elevation="2" rounded="10">
                <v-card-text>
                  <v-container fluid>
                    <v-row
                      v-for="face in media.faces"
                      :key="face.id"
                      class="face-row"
                      @click="editFace(face)"
                      :style="{ 'border-left-color': faceColor(face) }"
                    >
                      <v-col sm="2">
                        <img
                          class="face-image"
                          :src="'/api/media/thumbnail/' + face.thumbnail.id"
                        />
                      </v-col>
                      <v-col sm="10">
                        <v-row>
                          <v-col sm="12" class="pa-0">
                            <h1>
                              {{ faceTitle(face) }}
                            </h1>
                            <h4>{{ age(face) }}</h4>
                          </v-col>
                          <v-col sm="5">
                            <v-row
                              v-for="(item, i) in getFaceProperties(face)"
                              :key="i"
                            >
                              <v-col sm="4">{{ item.label }}</v-col>
                              <v-col sm="8" class="font-weight-bold">{{
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
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item value="objects"> Objects </v-tab-item>
            <v-tab-item value="raw">
              <div class="json-wrapper">
                <vue-json-pretty :data="media"> </vue-json-pretty>
              </div>
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

export default {
  components: {
    VueJsonPretty,
  },
  props: {
    media: Object,
  },
  data() {
    return {
      tab: "attributes",
    };
  },
  computed: {
    map: function () {
      let center = null;

      console.log(this.media);
      if (this.media.geoLocation) {
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
    location: function () {
      if (this.media.geoLocation) {
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
            value: loc.address.distic1,
          },
          {
            label: "District 2",
            value: loc.address.distic2,
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
      return [
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
          value: Math.round(this.media.size / 1024.0 / 1024.0, 2) + " MB",
        },
        {
          label: "Dimension",
          value: `${this.media.dimension.width} x ${this.media.dimension.height}`,
        },
        {
          label: "Camera",
          value: this.media.camera.make + " " + this.media.camera.model,
        },
        {
          label: "id",
          value: this.media.id,
        },
      ];
    },
  },
  methods: {
    getFaceProperties: function (face) {
      return [
        {
          label: "State / Type",
          value: `${face.state}`,
        },
      ];
    },
    back: function () {
      this.$emit("back");
    },
    faceTitle: function (face) {
      let name = "...";
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
  },
};
</script>

<style scoped>
.card-content {
  background-color: #1b1b1c;
  height: 100%;
}

.card-details-content {
  background-color: #494949;
  height: 100%;
  color: #fff;
}

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
  opacity: 0.3;
}

.face-image {
  width: 200px;
  height: 200px;
  border-radius: 100%;
}
</style>>
