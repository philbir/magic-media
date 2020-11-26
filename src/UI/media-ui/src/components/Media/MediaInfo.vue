<template>
  <v-card height="100%">
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
        <v-col sm="4"
          ><v-img :src="`/api/media/webimage/${media.id}`"></v-img
        ></v-col>
        <v-col sm="8">
          <v-tabs-items v-model="tab">
            <v-tab-item value="attributes" class="card-details-content">
              <v-row v-for="(item, i) in attributes" :key="i">
                <v-col cols="12" sm="2">{{ item.label }}</v-col>
                <v-col cols="12" sm="10" class="font-weight-bold">{{
                  item.value
                }}</v-col>
              </v-row>
            </v-tab-item>
            <v-tab-item value="location">
              <GmapMap
                :center="map.center"
                :zoom="10"
                :options="map.options"
                ref="mapRef"
                map-type-id="roadmap"
                style="width: 100%; height: 580px"
              >
                <GmapMarker v-if="map.center" :position="map.center"
              /></GmapMap>
            </v-tab-item>

            <v-tab-item value="faces">
              <media-quick-info :faces="media.faces"></media-quick-info>
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
import MediaQuickInfo from "./MediaQuickInfo";

export default {
  components: {
    VueJsonPretty,
    MediaQuickInfo,
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
        zoom: 7,
        options: {
          zoomControl: false,
          mapTypeControl: false,
          scaleControl: false,
          streetViewControl: false,
          rotateControl: false,
          fullscreenControl: false,
          disableDefaultUi: false,
        },
        marker: null,
      };
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
    back: function () {
      this.$emit("back");
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
  background-color: #1e1e1e;
  height: 100%;
  color: silver;
}

.json-wrapper {
  height: 80vh;
  overflow: auto;
}
</style>>
