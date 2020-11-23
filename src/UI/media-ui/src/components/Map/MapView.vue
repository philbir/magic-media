<template>
  <GmapMap
    :center="place"
    :zoom="7"
    ref="mapRef"
    map-type-id="terrain"
    style="width: 100%; height: 94vh"
    @zoom_changed="change"
    @bounds_changed="change"
    @center_changed="change"
  >
    <gmap-custom-marker
      v-for="(cluster, i) in mapClusters"
      :key="i"
      :marker="cluster.marker"
    >
      <div class="map-cluster">{{ cluster.count }}</div>
    </gmap-custom-marker>
  </GmapMap>
</template>

<script>
import zoomMap from "../../services/zoomMap";
import GmapCustomMarker from "vue2-gmap-custom-marker";
import { debounce } from "lodash";

export default {
  components: {
    "gmap-custom-marker": GmapCustomMarker,
  },
  created() {
    this.change = debounce(this.change, 1000);
  },
  mounted() {},
  data() {
    return {};
  },
  computed: {
    mapClusters: function () {
      return this.$store.state.map.clusters.map((x) => {
        return {
          marker: {
            lat: x.coordinates.latitude,
            lng: x.coordinates.longitude,
          },
          count: x.count,
        };
      });
    },
    place: function () {
      if (this.$store.state.map.place) {
        const loc = this.$store.state.map.place.geometry.location;
        return { lat: loc.lat(), lng: loc.lng() };
      } else {
        return { lat: 47.28752423541094, lng: 8.533110649108862 };
      }
    },
  },
  methods: {
    change: function () {
      var bounds = this.$refs.mapRef.$mapObject.getBounds();

      var sw = bounds.getSouthWest();
      var ne = bounds.getNorthEast();
      var zoom = this.$refs.mapRef.$mapObject.getZoom();

      const input = {
        box: {
          northEast: {
            latitude: ne.lat(),
            longitude: ne.lng(),
          },
          southWest: {
            latitude: sw.lat(),
            longitude: sw.lng(),
          },
        },
        precision: zoomMap[zoom].precision,
      };

      this.$store.dispatch("map/getClusters", input);
    },
  },
};
</script>

<style lang="scss">
$map-cluster-size: 24px;

.map-cluster {
  width: $map-cluster-size;
  height: $map-cluster-size;
  margin-left: $map-cluster-size / 2;
  margin-top: $map-cluster-size / 2;
  border-radius: 50%;
  font-size: 12px;
  color: #fff;
  line-height: $map-cluster-size;
  text-align: center;
  background: #0d2f66;
}
</style>
