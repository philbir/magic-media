<template>
  <GmapMap
    :center="{ lat: 47.28752423541094, lng: 8.533110649108862 }"
    :zoom="7"
    ref="mapRef"
    map-type-id="terrain"
    style="width: 1200px; height: 800px"
    @zoom_changed="change"
    @bounds_changed="change"
    @center_changed="change"
  >
  </GmapMap>
</template>

<script>
/* eslint-disable no-debugger */

export default {
  mounted() {
    // At this point, the child GmapMap has been mounted, but
    // its map has not been initialized.
    // Therefore we need to write mapRef.$mapPromise.then(() => ...)
    //this.$refs.mapRef.$mapPromise.then((map) => {
    //  map.panTo({lat: 1.38, lng: 103.80})
    //})
  },
  methods: {
    change: function () {
      var bounds = this.$refs.mapRef.$mapObject.getBounds();

      var sw = bounds.getSouthWest();
      var ne = bounds.getNorthEast();

      var zoom = this.$refs.mapRef.$mapObject.getZoom();
      console.log(sw, ne, zoom);

      //console.log(e);
    },
  },
};
</script>

<style>
</style>
