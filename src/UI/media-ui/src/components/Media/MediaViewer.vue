<template>
  <media-info
    v-if="showInfoPage"
    :mediaId="media.id"
    @back="showInfoPage = false"
  ></media-info>

  <div v-resize="onResize" v-else>
    <v-progress-linear v-if="loading" indeterminate color="blue" top />
    <div
      v-else
      class="media-wrapper"
      @dblclick="onLongpress"
      v-touch="{
        left: () => swipe('left'),
        right: () => swipe('right'),
        up: () => swipe('up'),
        down: () => swipe('down'),
      }"
    >
      <GlobalEvents
        @keydown="keyPressed"
        :filter="
          (event, handler, eventName) => event.target.tagName !== 'INPUT'
        "
      ></GlobalEvents>

      <div class="media-nav">
        <v-row>
          <v-col class="ma-4">
            <v-icon large color="grey lighten-2" @click="handlePrevious">
              mdi-chevron-left-circle
            </v-icon>
          </v-col>
          <v-spacer style="z-index: -1"></v-spacer>
          <v-col justify="end" align="right" class="ma-3">
            <v-icon large color="grey lighten-2" @click="handleNext">
              mdi-chevron-right-circle
            </v-icon>
          </v-col>
        </v-row>
      </div>
      <div class="head">
        <v-row>
          <v-col class="ml-2" xs="1" lg="9">
            <v-icon @click="handleHome" color="white" class="mr-2">
              mdi-home
            </v-icon>
            <span
              class="path d-none d-md-inline"
              v-for="(path, i) in pathInfo"
              :key="path.path"
              @click="setFolderFilter(path.path)"
              >{{ path.name }}
              <span v-if="i < pathInfo.length - 1"> | </span>
            </span>
            <span
              class="path d-none d-md-inline"
              v-if="media.dateTaken"
              @click="setDateFilter(media.dateTaken)"
            >
              @ {{ media.dateTaken | dateformat("DATE_MED") }}
            </span>
          </v-col>
          <v-spacer v-if="$vuetify.breakpoint.mdAndUp"></v-spacer>
          <v-col class="mr-1" xs="11" lg="2" align="right">
            <v-progress-circular
              indeterminate
              :size="22"
              :width="2"
              color="white"
              class="mr-2 mr-lg-4"
              v-show="headerLoading"
            ></v-progress-circular>
            <v-icon
              :color="media.isFavorite ? 'red' : 'white'"
              class="mr-2 mr-lg-4"
              @click="toggleFavorite(media)"
            >
              mdi-heart
            </v-icon>

            <v-icon class="mr-2 mr-lg-4" color="white" @click="toggleInfo">
              mdi-information-outline
            </v-icon>

            <viewer-menu
              @faceAction="onFaceAction"
              @mediaAction="onMediaAction"
            ></viewer-menu>
          </v-col>
        </v-row>
      </div>
      <div class="foot">
        {{ geoLocation }}
      </div>
      <img
        :src="imageSrc"
        @load="onImgLoaded"
        ref="img"
        v-if="media.mediaType === 'IMAGE'"
      />
      <div v-else class="video-wrapper">
        <vue-core-video-player
          :src="video.src"
          :muted="false"
        ></vue-core-video-player>
      </div>
      <div v-if="image.loaded && showFaceBox">
        <template v-for="face in media.faces">
          <face-box :key="face.id" :face="face" :image="image"></face-box>
        </template>
      </div>
      <AIObjects
        v-if="image.loaded && showObjects"
        :image="image"
        :objects="media.ai.objects"
      ></AIObjects>

      <div v-if="showQuickInfo" class="quick-info">
        <media-quick-info :faces="media.faces"></media-quick-info>
      </div>
    </div>
    <div class="filmstripe" v-show="showStripe">
      <FilmStripe></FilmStripe>
    </div>

    <div v-if="caption" class="caption" :style="{ color: '#' + caption.color }">
      <h2>{{ caption.text }}</h2>
    </div>
  </div>
</template>

<script>
import FilmStripe from "./FilmStripe.vue";
//import debounce from "lodash";
import { parsePath } from "../../services/mediaService";
import MediaQuickInfo from "./MediaQuickInfo.vue";
import MediaInfo from "./MediaInfo";
import FaceBox from "../Face/FaceBox";
import GlobalEvents from "vue-global-events";
import { DateTime } from "luxon";
import { mapActions, mapGetters } from "vuex";
import AIObjects from "./AIObjects";
import ViewerMenu from "./ViewerMenu";

export default {
  components: {
    FaceBox,
    FilmStripe,
    GlobalEvents,
    MediaQuickInfo,
    MediaInfo,
    AIObjects,
    ViewerMenu,
  },
  data() {
    return {
      image: {
        loaded: false,
        width: 0,
        naturalWidth: 0,
        offsetLeft: 0,
        offsetTop: 0,
      },
      showInfoPage: false,
      showStripe: false,
      mediaId: this.$route.params.id,
      windowWidth: window.innerWidth,
    };
  },
  mounted() {
    document.addEventListener("backbutton", this.handleHome, false);
    history.pushState(null, "Magic Media | view", location.href);
    window.addEventListener("popstate", this.handleHome);
  },
  beforeDestroy() {
    document.removeEventListener("backbutton", this.handleHome);
    document.removeEventListener("popstate", this.handleHome);
  },
  created() {
    //this.onResize = debounce(this.onResize, 1000);
    //this.onMouseMove = debounce(this.onMouseMove, 500);
  },
  computed: {
    ...mapGetters("user", ["userActions"]),
    headerLoading: function () {
      return this.$store.state.media.viewerHeaderLoading;
    },
    thumbnail: function () {
      if (this.$store) {
        const existing = this.$store.state.media.list.filter(
          (x) => x.id === this.$route.params.id
        );

        if (existing.length > 0) {
          return existing[0].thumbnail.dataUrl;
        }
      }

      return null;
    },
    pathInfo: function () {
      return parsePath(this.media.folder);
    },
    caption: function () {
      if (this.media.ai && this.media.ai.caption) {
        return {
          text: this.media.ai.caption.text,
          color: this.media.ai.colors.accent,
        };
      }

      return null;
    },
    video: function () {
      return {
        src: "/api/video/" + this.media.id,
      };
    },
    imageSrc: function () {
      return "/api/media/webimage/" + this.media.id;
    },
    loading: function () {
      return this.media === null;
    },
    media: function () {
      return this.$store.state.media.current;
    },
    showFaceBox: function () {
      return this.$store.state.media.viewer.showFaceBox;
    },
    showObjects: function () {
      return this.$store.state.media.viewer.showObjects;
    },
    showQuickInfo: function () {
      return this.$store.state.media.viewer.showFaceList;
    },
    geoLocation: function () {
      if (this.media.geoLocation && this.media.geoLocation.address) {
        return this.media.geoLocation.address.name;
      }
      return null;
    },
    box: function () {
      const media = this.media;

      if (media) {
        const w = document.querySelector("#app").clientWidth;
        const h = document.querySelector("#app").clientHeight;
        const screenOrientation = w > h ? "l" : "p";

        const box = { left: 0, top: 0 };

        if (screenOrientation === "l") {
          const ar = media.dimension.height / h;
          box.height = h;
          box.width = media.dimension.width / ar;
          box.left = (w - box.width) / 2;
        } else {
          const ar = media.dimension.width / w;
          box.width = w;
          box.height = media.dimension.height / ar;
          box.top = (h - box.height) / 2;
        }
        return box;
      }

      return null;
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    browserBackClicked: function (e) {
      e.preventDefault();
      return false;
    },
    onImgLoaded() {
      this.$nextTick(() => {
        window.setTimeout(() => {
          this.setImage();
        }, 500);
      });
    },
    setImage() {
      if (this.$refs.img && this.$refs.img.width) {
        this.image = {
          width: this.$refs.img.width,
          naturalWidth: this.$refs.img.naturalWidth,
          offsetLeft: this.$refs.img.offsetLeft - this.$refs.img.width / 2,
          offsetTop: this.$refs.img.offsetTop - this.$refs.img.height / 2,
          loaded: true,
        };
      }
    },
    async onLongpress() {
      this.$store.dispatch("media/share", [this.media]);
    },
    swipe: function (direction) {
      switch (direction) {
        case "left":
          this.navigate(+1);
          break;
        case "right":
          this.navigate(-1);
          break;
        case "down":
          this.handleHome();
          break;
        case "up":
          this.toggleFavorite();
          break;
      }
    },
    handlePrevious: function () {
      this.navigate(-1);
    },
    handleNext: function () {
      this.navigate(+1);
    },
    navigate: function (step) {
      this.image.loaded = false;
      var nextId = this.$store.getters["next"](step);
      if (nextId) {
        this.$store.dispatch("media/show", nextId);
      } else {
        this.handleHome();
      }
    },
    handleHome: function () {
      this.$store.dispatch("media/close");
    },
    onMouseMove: function (e) {
      if (this.media.mediaType === "IMAGE") {
        this.showStripe = e.clientY > 300;
      }
    },
    toggleFavorite: function () {
      this.$store.dispatch("media/toggleFavorite", this.media);
    },
    keyPressed: function (e) {
      switch (e.which) {
        case 37:
          this.navigate(-1);
          break;
        case 39:
          this.navigate(1);
          break;
        case 27:
          this.handleHome();
          break;
        case 46:
          this.recycle();
          break;
        case 65: //a
          this.approveAll();
          break;
        case 82: //r
          this.deleteUnassigned();
          break;
        case 85: //u
          this.unassignPredicted();
          break;
        case 80: //p
          this.predictPersons();
          break;
        case 70: //f
          this.toggleFavorite();
          break;
        case 67: //c
          this.analyseAI();
          break;
        case 32: //space
          this.$store.dispatch(
            "media/setViewerOptions",
            Object.assign({}, this.$store.state.media.viewer, {
              showFaceBox: !this.$store.state.media.viewer.showFaceBox,
            })
          );
          break;
        case 66: //b
          this.$store.dispatch(
            "media/setViewerOptions",
            Object.assign({}, this.$store.state.media.viewer, {
              showFaceList: !this.$store.state.media.viewer.showFaceList,
            })
          );
          break;
        case 79: //o
          this.$store.dispatch(
            "media/setViewerOptions",
            Object.assign({}, this.$store.state.media.viewer, {
              showObjects: !this.$store.state.media.viewer.showObjects,
            })
          );
          break;
        case 68: //d
          console.log(this.$vuetify.breakpoint);
          break;
        default:
          break;
      }
    },
    onResize() {
      this.setImage();
    },
    onFaceAction: function (action) {
      switch (action) {
        case "APPROVE_ALL":
          this.approveAll();
          break;
        case "UNASIGN_PREDICTED":
          this.unassignPredicted();
          break;
        case "PREDICT":
          this.predictPersons();
          break;
        case "DELETED_UNASSINGNED":
          this.deleteUnassigned();
          break;
      }
    },
    onMediaAction: function (action) {
      switch (action) {
        case "RECYCLE":
          this.recycle();
          break;
        case "AI":
          this.analyseAI();
          break;
      }
    },
    approveAll: function () {
      this.$store.dispatch("face/approveAllByMedia", this.media.id);
    },
    unassignPredicted: function () {
      this.$store.dispatch("face/unAssignPredictedByMedia", this.media.id);
    },
    deleteUnassigned: function () {
      this.$store.dispatch("face/deleteUnassignedByMedia", this.media.id);
    },
    predictPersons: function () {
      this.$store.dispatch("face/predictPersonsByMedia", this.media.id);
    },
    recycle() {
      this.$store.dispatch("media/recycle", [this.media.id]);
      this.navigate(1);
    },
    analyseAI() {
      this.$store.dispatch("media/analyseAI", this.media.id);
    },
    toggleInfo: function () {
      this.showInfoPage = !this.showInfoPage;
    },
    setFolderFilter: function (folder) {
      this.setFilter({
        key: "folder",
        value: folder,
      });
      this.handleHome();
    },
    setDateFilter: function (date) {
      var dateFilter = DateTime.fromISO(date).toISODate();
      this.setFilter({
        key: "date",
        value: dateFilter,
      });
      this.handleHome();
    },
  },
};
</script>

<style scoped>
.media-wrapper {
  background-color: #1b1b1c;
  height: 100vh;
}
.media-wrapper img {
  height: 100vh;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.media-nav {
  position: absolute;
  display: flex;
  width: 99%;
  top: 44vh;
  z-index: 100;
}

.head {
  color: #fff;
  line-height: 20px;
  position: absolute;
  padding: 0;
  width: 100%;
  left: 0;
  top: 0;
  background-color: #000;
  opacity: 0.5;
  z-index: 100;
  height: 48px;
}

.foot {
  color: #fff;
  position: absolute;
  bottom: 0;
  padding: 4px;
  z-index: 10;
  background-color: #000;
  opacity: 0.5;
  width: 100%;
}

.caption {
  position: absolute;
  top: 48px;
  left: 10px;
  margin: auto;
  text-align: center;
}

.filmstripe {
  position: absolute;
  overflow-x: hidden;
  overflow-y: hidden;
  white-space: nowrap;
  bottom: 50px;
  width: 100%;
  z-index: 10;
  z-index: 10;
}

.video-wrapper {
  height: 94vh;
  z-index: 50;
}

.path {
  cursor: pointer;
  color: #c0c0c0;
  transition: color 300ms ease-in;
  overflow: hidden;
  text-overflow: ellipsis;
}

.path:hover {
  color: #fff;
}

.quick-info {
  position: absolute;
  top: 50px;
  right: 40px;
}
</style>

