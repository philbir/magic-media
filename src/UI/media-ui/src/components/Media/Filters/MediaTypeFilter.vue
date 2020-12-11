<template>
  <div class="ml-6">
    <v-switch v-model="image" color="primary" label="Images"></v-switch>
    <v-switch v-model="video" color="primary" label="Videos"></v-switch>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  computed: {
    image: {
      set(value) {
        this.setMediaTypeFilter(value, this.video);
      },
      get() {
        return (
          this.$store.state.media.filter.mediaTypes.length === 0 ||
          this.$store.state.media.filter.mediaTypes.includes("IMAGE")
        );
      },
    },
    video: {
      set(value) {
        this.setMediaTypeFilter(this.image, value);
      },
      get() {
        return (
          this.$store.state.media.filter.mediaTypes.length === 0 ||
          this.$store.state.media.filter.mediaTypes.includes("VIDEO")
        );
      },
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    setMediaTypeFilter(image, video) {
      const selected = [];
      if (image) {
        selected.push("IMAGE");
      }
      if (video) {
        selected.push("VIDEO");
      }
      this.setFilter({
        key: "mediaTypes",
        value: selected.length == 0 ? [] : selected,
      });
    },
  },
};
</script>

<style>
</style>