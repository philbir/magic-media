<template>
  <div>
    <div
      class="media-container"
      :style="{ height: layout.containerHeight + 20 + 'px' }"
    >
      <div
        v-for="(box, i) in layout.boxes"
        :key="i"
        class="media-item"
        v-on:click="
          $router.push({ name: 'media', params: { id: box.media.id } })
        "
        :style="{
          left: box.left + 'px',
          top: box.top + 'px',
          height: box.height + 'px',
          width: box.width + 'px',
          'background-image': 'url(' + box.media.thumbnail.dataUrl + ')',
        }"
      />
    </div>

    <v-icon large color="gray" @click="handleUpload"> mdi-upload </v-icon>
    <v-icon large color="gray" @click="handleRefresh"> mdi-refresh </v-icon>
  </div>

  <!-- No result -->
</template>

<script>
import justified from "justified-layout";
import { mediaListViewMap } from "../services/mediaListViewMap";

export default {
  created() {
    if (this.$store.state.mediaList.length === 0)
      this.$store.dispatch("searchMedia");
  },
  computed: {
    windowWidth: () => window.innerWidth,
    layout: function () {
      const items = this.$store.state.mediaList;
      const viewMap = mediaListViewMap["l"];
      const ratios = [];
      items.forEach((item) => {
        ratios.push(
          viewMap.fixedRatio > 0
            ? viewMap.fixedRatio
            : item.dimension.width / item.dimension.height
        );
      });
      const layout = justified(ratios, {
        containerWidth: window.innerWidth,
        targetRowHeight: viewMap.rowHeight,
        boxSpacing: viewMap.spacing,
        containerPadding: viewMap.spacing,
      });

      layout.boxes.forEach((box, i) => {
        box.media = items[i];
      });
      return layout;
    },
  },
  data() {
    return {};
  },
  methods: {
    handleUpload() {
      this.$router.push("/upload");
    },
    handleRefresh() {
      this.$store.dispatch("searchMedia");
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
