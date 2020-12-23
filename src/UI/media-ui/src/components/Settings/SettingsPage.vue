<template>
  <div class="ma-8">
    <h2>Settings</h2>

    <v-btn @click="buildModel"> Build Face model </v-btn>

    <v-switch
      v-model="loadThumbData"
      color="primary"
      label="Load thumbnail data"
    ></v-switch>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  computed: {
    loadThumbData: {
      set() {
        this.toggleLoadThumbnailData();
      },
      get() {
        return this.$store.state.media.loadThumbnailData;
      },
    },
  },
  methods: {
    ...mapActions("person", ["buildModel"]),
    ...mapActions("media", ["toggleLoadThumbnailData"]),
  },
};
</script>

<style lang="scss" scoped>
</style>>
