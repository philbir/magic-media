<template>
  <FilterList
    :items="cameras"
    title="Camera"
    max-height="250"
    v-model="selectedCameras"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedCameras: {
      set(value) {
        this.setFilter({
          key: "cameras",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.cameras;
      },
    },
    cameras: function () {
      return this.$store.state.media.facets.camera;
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
  },
};
</script>

<style>
</style>