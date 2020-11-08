<template>
  <div
    class="face-container"
    ref="container"
    v-scroll.self="onScroll"
    v-if="layout.clientHeight > 0"
    :style="{ height: layout.clientHeight + 'px' }"
  >
    <v-progress-linear v-if="loading" indeterminate color="blue" top />

    <template v-for="(row, i) in layout.rows">
      <div
        :key="i"
        v-if="inViewPort(row.top)"
        class="face-row visible"
        :style="{
          top: row.top + 'px',
        }"
      >
        <div class="face-row-container">
          <div
            class="face-item"
            :class="{ selected: isSelected(face.idx) }"
            v-for="(face, i) in row.faces"
            :key="i"
            v-on:click="clickFace(face)"
          >
            <div
              class="face-item-image"
              :style="{
                'background-image': 'url(' + face.thumbnail.dataUrl + ')',
                'border-color': face.color,
              }"
            ></div>
            <div class="face-item-title">
              {{ face.person ? face.person.name : "" }}
            </div>
          </div>
        </div>
      </div>
      <div
        :key="i"
        v-else
        :style="{
          top: row.top + 'px',
          height: row.height + 'px',
        }"
        class="face-row"
      ></div>
    </template>
  </div>
</template>

<script>
import { debounce, chunk } from "lodash";
import { getFaceColor } from "../services/faceColor";

export default {
  components: {},
  created() {
    if (this.$store.state.face.list.length === 0)
      this.$store.dispatch("face/search");

    this.containerWith =
      window.innerWidth - (this.$vuetify.breakpoint.mobile ? 0 : 264);
    this.onScroll = debounce(this.onScroll, 100);
  },

  computed: {
    layout: function () {
      const items = this.$store.state.face.list.map((item, i) => {
        item.color = getFaceColor(item);
        item.idx = i;
        return item;
      });
      const width = 112;
      const height = 130;
      const itemsPerRow = Math.floor(this.containerWith / width);

      const chunks = chunk(items, itemsPerRow);
      const rows = [];
      for (let i = 0; i < chunks.length; i++) {
        rows.push({
          top: i * height,
          height: height,
          faces: chunks[i],
        });
      }
      const layout = {
        clientHeight: window.innerHeight - 50,
        rows: rows,
        itemHeight: height,
      };
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
      tooltip: {
        show: false,
        activator: null,
      },
    };
  },
  methods: {
    inViewPort: function (top) {
      return top >= this.viewPort.start && top <= this.viewPort.end;
    },
    isSelected: function (idx) {
      if (this.$store.state.face.selectedIndexes.includes(idx)) {
        return true;
      }
      return false;
    },
    thumbSrc: function (media) {
      return media.thumbnail.dataUrl;
    },
    color: function (face) {
      return getFaceColor(face);
    },
    clickFace: function (face) {
      if (this.$store.state.face.isEditMode) {
        this.$store.dispatch("face/select", face.idx);
      } else {
        this.$store.dispatch("media/show", face.media.id);
      }
    },
    handleRefresh: function () {
      this.$store.dispatch("face/search");
    },
    loadMore() {
      this.$store.dispatch("face/loadMore");
    },

    selectAll: function () {
      this.$store.dispatch("face/selectAll");
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
.face-container {
  position: relative;
  overflow-y: auto;
  overflow-x: hidden;
}
.face-item {
  width: 112px;
  height: 120px;
}

.face-item-image {
  background-repeat: no-repeat;
  background-size: 100% 100%;
  border-radius: 3px;
  border-radius: 100%;
  border-style: solid;
  border-width: 3px;
  height: 100px;
  width: 100px;
  margin: auto;
}

.face-item-title {
  font-size: 14px;
  text-align: center;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.face-row {
  position: absolute;
}

.face-row-container {
  position: relative;
  display: flex;
  align-items: flex-start;
  align-content: space-around;
}

.face-item.selected {
  transform: scale(0.95);
  transition: opacity, transform 150ms ease-in-out;
  box-shadow: 4px 4px 2px 1px#c0bdbd;
}

.visible {
  transition: opacity 250ms ease-in;
}
</style>
