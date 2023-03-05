<template>
  <div
    class="media-container"
    ref="container"
    v-scroll.self="onScroll"
    :style="{ height: layout.clientHeight + 'px' }"
    v-longpress="longpress"
  >
    <v-progress-linear v-if="loading" indeterminate color="blue" top />
    <v-row class="filter-row">
      <v-chip
        v-show="filterDesc.length > 1"
        close
        small
        class="ma-2"
        text-color="white"
        color="blue darken-4"
        @click:close="resetAllFilters"
      >
        Clear all
      </v-chip>

      <v-chip
        v-for="(desc, i) in filterDesc"
        close
        small
        :key="i"
        class="ma-2"
        text-color="white"
        color="blue darken-4"
        @click:close="removeFilter(desc.key)"
      >
        <strong>{{ desc.name }}</strong
        >: {{ desc.description }}
      </v-chip>
    </v-row>
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
          v-on:click="selectMedia(box.media, $event)"
          @mouseover="hover(true, box.media.idx)"
          @mouseleave="hover(false, box.media.idx)"
          :style="{
            left: box.left + 'px',
            height: box.height + 'px',
            width: box.width + 'px',
            'background-image':
              layout.type === 'image' ? 'url(' + box.media.imageUrl + ')' : null
          }"
        >
          <div
            v-if="box.media.mediaType === 'VIDEO' && layout.type === 'image'"
            class="duration"
          >
            {{ box.media.videoInfo.duration }}
          </div>

          <div v-if="layout.type === 'detail'" class="media-detail-wrapper">
            <div
              class="media-detail-image"
              :style="{
                height: box.height + 'px',
                width: '100px',
                'background-image': 'url(' + box.media.imageUrl + ')'
              }"
            ></div>
            <div
              style="
                padding-left: 6px;
                font-size: 12px;
                background-color: #fff;"
              :style="{
                width: box.width - 100 + 'px'
              }"
            >
              <div class="detail-row">
                <strong>{{ box.media.filename }}</strong>
              </div>
              <div class="detail-row">{{ box.media.folder }}</div>
              <div class="detail-row">
                {{ box.media.camera ? box.media.camera.title : "" }}
              </div>
              <div class="detail-row">
                {{ box.media.dateTaken | dateformat("DATETIME_SHORT") }}
              </div>
              <div class="detail-row" :title="box.media.source.identifier">
                {{ box.media.source.identifier }}
              </div>
              <div>
                <small>{{ box.media.id }}</small>
              </div>
            </div>
            <div />
          </div>

          <div v-if="layout.type === 'table'" class="media-detail-wrapper">
            <div
              class="media-detail-image"
              :style="{
                height: box.height + 'px',
                width: '40px',
                'background-image': 'url(' + box.media.imageUrl + ')'
              }"
            ></div>

            <div class="table-column" style="width: 200px">
              <strong> {{ box.media.filename }}</strong>
            </div>
            <div class="table-column" style="width: 200px">
              {{ box.media.folder }}
            </div>
            <div class="table-column" style="width: 150px">
              {{ box.media.camera ? box.media.camera.title : "" }}
            </div>
            <div class="table-column" style="width: 150px">
              {{ box.media.dateTaken | dateformat("DATETIME_SHORT") }}
            </div>

            <div class="table-column" style="width: 220px">
              {{ box.media.id }}
            </div>
            <div class="table-column" :title="box.media.source.identifier">
              {{ box.media.source.identifier }}
            </div>
          </div>
        </div>
      </div>
      <div
        :key="i + ''"
        v-else
        :style="{ top: row.top + 'px', height: row.boxes[0].height + 'px' }"
        class="media-row hidden"
      />
    </template>
  </div>
</template>

<script>
import justified from "justified-layout";
import { mediaListViewMap } from "../../services/mediaListViewMap";
import { debounce } from "lodash";
import { mapActions, mapGetters } from "vuex";

export default {
  components: {},
  created() {
    if (
      this.$store.state.media.list.length === 0 &&
      !this.$store.state.media.listLoading
    )
      this.$store.dispatch("media/search");

    this.containerWith =
      window.innerWidth - (this.$vuetify.breakpoint.mobile ? 0 : 264);
    this.onScroll = debounce(this.onScroll, 50);
  },
  computed: {
    ...mapGetters("user", ["userActions"]),
    layout: function() {
      const items = this.$store.state.media.list;
      const viewMap = mediaListViewMap[this.$store.state.media.thumbnailSize];
      const ratios = [];
      items.forEach(item => {
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
        containerPadding: viewMap.spacing
      });
      layout.withText = viewMap.withText;
      layout.type = viewMap.layoutType;
      layout.rowHeight = viewMap.rowHeight + viewMap.spacing;
      layout.clientHeight = window.innerHeight - 50;
      layout.boxes.forEach((box, i) => {
        box.media = items[i];
        box.media.imageUrl = this.thumbSrc(items[i]);
        box.media.idx = i;
      });
      layout.rows = [];
      let currentTop = 0;
      let currentRow = {};
      for (let i = 0; i < layout.boxes.length; i++) {
        if (layout.boxes[i].top !== currentTop) {
          currentRow = {
            top: layout.boxes[i].top,
            boxes: []
          };
          layout.rows.push(currentRow);
          currentTop = layout.boxes[i].top;
        }
        currentRow.boxes.push(layout.boxes[i]);
      }
      layout.boxes = null;

      return layout;
    },
    loading: function() {
      return this.$store.state.media.listLoading;
    },
    filterDesc: function() {
      return this.$store.getters["media/filterDescriptions"];
    }
  },
  data() {
    return {
      containerWith: 0,
      viewPort: {
        start: 0,
        end: window.innerHeight
      },
      hoveredIndex: -1,
      previousScroll: 0
    };
  },
  methods: {
    ...mapActions("media", ["removeFilter", "resetAllFilters"]),
    inViewPort: function(top) {
      return top > this.viewPort.start && top < this.viewPort.end;
    },
    isSelected: function(idx) {
      if (this.$store.state.media.selectedIndexes.includes(idx)) {
        return true;
      }
      return false;
    },
    thumbSrc: function(media) {
      if (media.mediaType === "VIDEO" && this.hoveredIndex === media.idx) {
        return "/api/video/preview/" + media.id;
      }
      return media.thumbnail ? media.thumbnail.dataUrl : null;
    },
    handleUpload: function() {
      this.$router.push("/upload");
    },
    handleRefresh: function() {
      this.$store.dispatch("media/search");
    },
    loadMore() {
      this.$store.dispatch("media/loadMore");
    },
    selectMedia: function(media, e) {
      if (this.$store.state.media.isEditMode) {
        this.$store.dispatch("media/select", {
          idx: media.idx,
          multi: e.shiftKey
        });
      } else {
        if (e.shiftKey && this.userActions.media.edit) {
          this.$store.dispatch("media/toggleEditMode", true);
          this.$store.dispatch("media/select", {
            idx: media.idx,
            multi: false
          });
        } else if (
          e.ctrlKey &&
          this.userActions.media.edit &&
          this.$store.state.media.filter.albumId
        ) {
          this.$store.dispatch("album/setCover", {
            albumId: this.$store.state.media.filter.albumId,
            mediaId: media.id
          });
        } else {
          this.$store.dispatch("media/show", media.id);
        }
      }
    },
    selectAll: function() {
      this.$store.dispatch("media/selectAll");
    },
    hover: function(isHover, index) {
      if (isHover) {
        this.hoveredIndex = index;
      } else {
        this.hoveredIndex = -1;
      }
    },
    onScroll: function(e) {
      const elm = e.target;
      const percent = (elm.scrollTop + elm.clientHeight) / elm.scrollHeight;
      const offset = 600;
      this.viewPort = {
        start: elm.scrollTop - offset,
        end: elm.scrollTop + elm.offsetHeight + offset
      };
      if (!this.loading && percent > 0.85 && percent > this.previousScroll) {
        this.loadMore();
      }

      this.previousScroll = percent;
    },
    longpress: function() {
      this.$store.dispatch("openNavDrawer", true);
    }
  }
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
  background-color: #ececec;
}

.media-row {
  position: absolute;
}

.hidden::after {
  content: ".";
  visibility: hidden;
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
  transition: opacity 50ms ease-in;
}

.duration {
  color: #fff;
  margin-left: 4px;
}

.filter-row {
  z-index: 2;
  opacity: 0.7;
  width: 100%;
  position: absolute;
  margin: 0;
  height: 36px;
}

.media-detail-wrapper {
  display: flex;
  font-size: 12px;
}

.media-detail-image {
  background-repeat: no-repeat;
  background-size: 100% 100%;
}

.detail-row {
  border-bottom: 1px solid #d0d0d0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.table-column {
  padding-left: 4px;
  border-left: 2px solid #fff;
  margin-top: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
