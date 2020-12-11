<template>
  <div>
    <GmapMap
      :center="mapCenter"
      :zoom="7"
      :options="mapOptions"
      ref="mapRef"
      map-type-id="roadmap"
      style="width: 100%; height: 200px"
      @click="clickMap"
    >
      <GmapMarker v-if="mapMarker" :position="mapMarker"
    /></GmapMap>

    <v-text-field
      class="ml-4 mr-4"
      v-model="mapRadius"
      label="Radius"
      suffix="km"
      @change="onRadiusChange"
      prepend-inner-icon="mdi-map-marker-radius-outline"
    ></v-text-field>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  data() {
    return {
      mapOptions: {
        zoomControl: false,
        mapTypeControl: false,
        scaleControl: false,
        streetViewControl: false,
        rotateControl: false,
        fullscreenControl: false,
        disableDefaultUi: false,
      },

      mapMarker: null,
      mapRadius: 10,
    };
  },
  computed: {
    mapCenter: function () {
      return { lat: 47.28752423541094, lng: 8.533110649108862 };
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    clickMap: function (loc) {
      this.mapMarker = {
        lat: loc.latLng.lat(),
        lng: loc.latLng.lng(),
      };

      this.setSearchFilter();
    },
    onRadiusChange: function () {
      this.setSearchFilter();
    },
    setSearchFilter: function () {
      if (this.mapMarker) {
        const geoFilter = {
          latitude: this.mapMarker.lat,
          longitude: this.mapMarker.lng,
          distance: +this.mapRadius,
        };
        this.setFilter({
          key: "geoRadius",
          value: geoFilter,
        });
      }
    },
  },
};
</script>

<style>
</style>