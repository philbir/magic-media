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
            v-on:click="clickFace(face, $event)"
          >
            <div
              class="face-item-image"
              :style="{
                'background-image': 'url(' + thumbSrc(face) + ')',
                'border-color': face.color,
              }"
            ></div>
            <div class="face-item-title">
              {{ face.title }}
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
        class="face-row hidden"
      />
    </template>
  </div>
</template>

<script>
import { debounce, chunk } from "lodash";
import { getFaceColor } from "../../services/faceColor";

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
        item.title = getTitle(item);
        return item;
      });
      const width = 132;
      const height = 150;
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
      previousScroll: 0,
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
    thumbSrc: function (face) {
      if (face.thumbnail.dataUrl) {
        return face.thumbnail.dataUrl;
      } else {
        return `/api/thumbnail/face/${face.id}`;
      }
    },
    color: function (face) {
      return getFaceColor(face);
    },
    clickFace: function (face, e) {
      if (this.$store.state.face.isEditMode) {
        this.$store.dispatch("face/select", {
          idx: face.idx,
          multi: e.shiftKey,
        });
      } else {
        if (e.ctrlKey) {
          this.$store.dispatch("face/openEdit", face);
        } else {
          if (e.shiftKey) {
            this.$store.dispatch("face/toggleEditMode", true);
            this.$store.dispatch("face/select", {
              idx: face.idx,
              multi: false,
            });
          } else {
            this.$store.dispatch("media/show", face.media.id);
          }
        }
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

      if (!this.loading && percent > 0.85 && percent > this.previousScroll) {
        this.loadMore();
      }

      this.previousScroll = percent;
    },
  },
};

const getTitle = (face) => {
  if (face.person) {
    let name = face.person.name;

    if (face.age) {
      name += ` (${Math.floor(face.age / 12)})`;
    }

    return name;
  }
  return null;
};
</script>

<style scoped>
.face-container {
  position: relative;
  overflow-y: auto;
  overflow-x: hidden;
}
.face-item {
  width: 132px;
  height: 140px;
}

.face-item-image {
  background-repeat: no-repeat;
  background-size: 100% 100%;
  border-radius: 3px;
  border-radius: 100%;
  border-style: solid;
  border-width: 3px;
  height: 120px;
  width: 120px;
  margin: auto;
  background-color: #e8e8e8;
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

.hidden::after {
  content: ".";
  visibility: hidden;
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
