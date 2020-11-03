<template>
  <div
    class="media-container"
    ref="container"
    :style="{ height: layout.containerHeight + 'px' }"
  >
    <v-progress-linear v-if="loading" indeterminate color="blue" top />
    <div
      v-for="(box, i) in layout.boxes"
      :key="i"
      class="media-item"
      v-on:click="$router.push({ name: 'media', params: { id: box.media.id } })"
      :style="{
        left: box.left + 'px',
        top: box.top + 'px',
        height: box.height + 'px',
        width: box.width + 'px',
        'background-image': 'url(' + box.media.thumbnail.dataUrl + ')',
      }"
    />
  </div>
  <!--
    <v-icon large color="gray" @click="handleUpload"> mdi-upload </v-icon>
    <v-icon large color="gray" @click="handleRefresh"> mdi-refresh </v-icon>
    -->

  <!-- No result -->
</template>

<script>
import justified from "justified-layout";
import { mediaListViewMap } from "../services/mediaListViewMap";

export default {
  created() {
    if (this.$store.state.media.list.length === 0)
      this.$store.dispatch("media/search");

    //this.containerWith = this.$refs.container.clientWidth - 5;
    this.containerWith = window.innerWidth - 264;
  },

  computed: {
    layout: function () {
      const items = this.$store.state.media.list;
      const viewMap =
        mediaListViewMap[this.$store.state.media.filter.thumbnailSize];
      const ratios = [];
      items.forEach((item) => {
        ratios.push(
          viewMap.fixedRatio > 0
            ? viewMap.fixedRatio
            : item.dimension.width / item.dimension.height
        );
      });
      const layout = justified(ratios, {
        containerWidth: this.containerWith,
        targetRowHeight: viewMap.rowHeight,
        boxSpacing: viewMap.spacing,
        containerPadding: viewMap.spacing,
      });

      layout.boxes.forEach((box, i) => {
        box.media = items[i];
      });
      return layout;
    },
    loading: function () {
      return this.$store.state.media.listLoading;
    },
  },
  data() {
    return {
      containerWith: 0,
    };
  },
  methods: {
    handleUpload: function () {
      this.$router.push("/upload");
    },
    handleRefresh: function () {
      this.$store.dispatch("media/search");
    },
  },
};
</script>

<style scoped>
.media-container {
  position: relative;
  overflow: hidden;
}
.media-item {
  position: absolute;
  background-repeat: no-repeat;
  background-size: 100% 100%;
}
</style>
