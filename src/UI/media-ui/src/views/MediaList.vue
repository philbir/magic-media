<template>
  <div
    class="media-container"
    ref="container"
    v-scroll.self="onScroll"
    :style="{ height: layout.clientHeight + 'px' }"
  >
    <v-progress-linear v-if="loading" indeterminate color="blue" top />

    <template v-for="(row, i) in layout.rows">
      <div
        :key="i"
        v-if="inViewPort(row.top)"
        class="media-row visible"
        :style="{ top: row.top + 'px' }"
      >
        <div
          v-for="(box, i) in row.boxes"
          :key="i"
          class="media-item"
          :class="{ selected: isSelected(box.media.idx) }"
          v-on:click="selectMedia(box.media)"
          :style="{
            left: box.left + 'px',
            height: box.height + 'px',
            width: box.width + 'px',
            'background-image': 'url(' + thumbSrc(box.media) + ')',
          }"
        ></div>
      </div>
      <div
        :key="i"
        v-else
        :style="{ top: row.top + 'px', height: row.boxes[0].height + 'px' }"
        class="media-row"
      >
        <h1 class="white--text">ss</h1>
      </div>
    </template>
  </div>
</template>

<script>
import justified from "justified-layout";
import { mediaListViewMap } from "../services/mediaListViewMap";
import { debounce } from "lodash";

export default {
  components: {},
  created() {
    if (this.$store.state.media.list.length === 0)
      this.$store.dispatch("media/search");

    this.containerWith = window.innerWidth - 264;
    this.onScroll = debounce(this.onScroll, 100);
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
      layout.rowHeight = viewMap.rowHeight + viewMap.spacing;
      layout.clientHeight = window.innerHeight - 50;
      layout.boxes.forEach((box, i) => {
        box.media = items[i];
        box.media.idx = i;
      });
      layout.rows = [];
      let currentTop = 0;
      let currentRow = {};
      for (let i = 0; i < layout.boxes.length; i++) {
        if (layout.boxes[i].top !== currentTop) {
          currentRow = {
            top: layout.boxes[i].top,
            boxes: [],
          };
          layout.rows.push(currentRow);
          currentTop = layout.boxes[i].top;
        }
        currentRow.boxes.push(layout.boxes[i]);
      }
      layout.boxes = null;

      return layout;
    },
    loading: function () {
      return this.$store.state.media.listLoading;
    },
  },
  data() {
    return {
      containerWith: 0,
      viewPort: {
        start: 0,
        end: window.innerHeight,
      },
    };
  },
  methods: {
    inViewPort: function (top) {
      return top > this.viewPort.start && top < this.viewPort.end;
    },
    isSelected: function (idx) {
      if (this.$store.state.media.selectedIndexes.includes(idx)) {
        return true;
      }
      return false;
    },
    thumbSrc: function (media) {
      return media.thumbnail.dataUrl;
    },
    handleUpload: function () {
      this.$router.push("/upload");
    },
    handleRefresh: function () {
      this.$store.dispatch("media/search");
    },
    loadMore() {
      this.$store.dispatch("media/loadMore");
    },
    selectMedia: function (media) {
      if (this.$store.state.media.isEditMode) {
        this.$store.dispatch("media/select", media.idx);
      } else {
        this.$store.dispatch("media/show", media.id);
      }
    },
    selectAll: function () {
      this.$store.dispatch("media/selectAll");
    },
    onScroll: function (e) {
      const elm = e.target;
      const percent = (elm.scrollTop + elm.clientHeight) / elm.scrollHeight;
      const offset = 600;
      this.viewPort = {
        start: elm.scrollTop - offset,
        end: elm.scrollTop + elm.offsetHeight + offset,
      };

      if (!this.loading && percent > 0.7) {
        this.loadMore();
      }
    },
  },
};
</script>

<style scoped>
.media-container {
  position: relative;
  overflow-y: auto;
  overflow-x: hidden;
}
.media-item {
  position: absolute;
  background-repeat: no-repeat;
  background-size: 100% 100%;
  border-radius: 3px;
}

.media-row {
  position: absolute;
}

.media-item.selected {
  transform: rotate(2deg);
  transform: scale(0.82);
  transition: opacity, transform 150ms ease-in-out;
  border-radius: 10px;
  border: 5px solid #66bb6a;
  box-shadow: 4px 4px 2px 1px#e4e4e4;
}

.visible {
  transition: opacity 250ms ease-in;
}
</style>
