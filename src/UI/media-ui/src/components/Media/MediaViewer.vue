<template>
  <media-info
    v-if="showInfoPage"
    :mediaId="media.id"
    @back="showInfoPage = false"
  ></media-info>

  <div v-resize="onResize" v-else>
    <v-progress-linear v-if="loading" indeterminate color="blue" top />
    <div v-else class="media-wrapper">
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
          <v-col class="ml-2" sm="9">
            <v-icon @click="handleHome" color="white" class="mr-2">
              mdi-home
            </v-icon>
            <span class="path" v-for="(path, i) in pathInfo" :key="path.path"
              >{{ path.name }}
              <span v-if="i < pathInfo.length - 1"> | </span>
            </span>
            <span class="path" v-if="media.dateTaken">
              @ {{ media.dateTaken | dateformat("DATE_MED") }}
            </span>
          </v-col>
          <v-spacer></v-spacer>
          <v-col class="mr-4" sm="2" align="right">
            <v-icon
              :color="media.isFavorite ? 'red' : 'white'"
              class="mr-4"
              @click="toggleFavorite(media)"
            >
              mdi-heart
            </v-icon>

            <v-icon class="mr-4" color="white" @click="toggleInfo">
              mdi-information-outline
            </v-icon>

            <v-menu
              :close-on-content-click="false"
              open-on-hover
              bottom
              offset-y
            >
              <template v-slot:activator="{ on, attrs }">
                <v-icon v-bind="attrs" v-on="on" color="white">
                  mdi-cog-outline
                </v-icon>
              </template>

              <v-list>
                <v-list-group prepend-icon="mdi-face-recognition">
                  <template v-slot:activator>
                    <v-list-item>
                      <v-list-item-content>
                        <v-list-item-title>Faces</v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>

                  <template v-for="item in settingsMenu.face.items">
                    <v-list-item
                      :key="item.title"
                      @click="onClickFaceAction(item.value)"
                    >
                      <v-list-item-icon>
                        <v-icon v-text="item.icon"></v-icon>
                      </v-list-item-icon>
                      <v-list-item-content>
                        <v-list-item-title
                          v-text="item.title"
                        ></v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>
                </v-list-group>
                <v-list-group prepend-icon="mdi-image-edit">
                  <template v-slot:activator>
                    <v-list-item>
                      <v-list-item-content>
                        <v-list-item-title>Actions</v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>

                  <template v-for="item in settingsMenu.actions.items">
                    <v-list-item
                      :key="item.title"
                      @change="onActionClick(item.action)"
                    >
                      <v-list-item-icon>
                        <v-icon v-text="item.icon"></v-icon>
                      </v-list-item-icon>
                      <v-list-item-content>
                        <v-list-item-title
                          v-text="item.title"
                        ></v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>
                </v-list-group>

                <v-list-group prepend-icon="mdi-eye-settings-outline">
                  <template v-slot:activator>
                    <v-list-item>
                      <v-list-item-content>
                        <v-list-item-title>View settings</v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>
                  <v-list-item-group
                    v-model="settingsMenu.viewer.selected"
                    @change="onViewOptionsChange"
                    multiple
                  >
                    <template v-for="item in settingsMenu.viewer.items">
                      <v-list-item :key="item.title" :value="item.value">
                        <template v-slot:default="{ active }">
                          <v-list-item-content>
                            <v-list-item-title
                              v-text="item.title"
                            ></v-list-item-title>
                          </v-list-item-content>

                          <v-list-item-action>
                            <v-list-item-action-text
                              v-text="item.action"
                            ></v-list-item-action-text>

                            <v-switch :input-value="active"></v-switch>
                          </v-list-item-action>
                        </template>
                      </v-list-item>
                    </template>
                  </v-list-item-group>
                </v-list-group>
              </v-list>
            </v-menu>
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
      <div v-if="showQuickInfo" class="quick-info">
        <media-quick-info :faces="media.faces"></media-quick-info>
      </div>
    </div>
    <div class="filmstripe" v-show="showStripe">
      <FilmStripe></FilmStripe>
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

export default {
  components: {
    FaceBox,
    FilmStripe,
    GlobalEvents,
    MediaQuickInfo,
    MediaInfo,
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
      settingsMenu: {
        viewer: {
          selected: ["SHOW_FACE_LIST"],
          items: [
            { title: "Face boxes", value: "SHOW_FACE_BOXES" },
            { title: "Face list", value: "SHOW_FACE_LIST" },
            { title: "Film stripe", value: "SHOW_FILM_STRIPE" },
            { title: "Objects", value: "SHOW_OBJECTS" },
          ],
        },
        face: {
          selected: [],
          items: [
            {
              title: "Approve all [a]",
              icon: "mdi-check-all",
              value: "APPROVE_ALL",
            },
            {
              title: "Unassign predicted [u]",
              icon: "mdi-close-outline",
              value: "UNASIGN_PREDICTED",
            },
            {
              title: "Remove unassigned  [r]",
              icon: "mdi-trash-can-outline",
              value: "DELETED_UNASSINGNED",
            },
          ],
        },
        actions: {
          selected: [],
          items: [
            { title: "Move", icon: "mdi-file-move-outline", action: "MOVE" },
            { title: "Edit", icon: "mdi-pencil", action: "EDIT" },
            { title: "Delete", icon: "mdi-recycle", action: "RECYCLE" },
            { title: "Add to Album", icon: "mdi-plus", action: "ADD_TO_ALBUM" },
          ],
        },
      },
      showStripe: false,
      mediaId: this.$route.params.id,
      windowWidth: window.innerWidth,
    };
  },

  created() {
    //this.onResize = debounce(this.onResize, 1000);
    //this.onMouseMove = debounce(this.onMouseMove, 500);
    this.setViewOptionsSelected(this.viewerOptions);
  },
  watch: {
    viewerOptions: function (newValue) {
      this.setViewOptionsSelected(newValue);
    },
  },
  computed: {
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
    showQuickInfo: function () {
      return this.$store.state.media.viewer.showFaceList;
    },
    viewerOptions: function () {
      return this.$store.state.media.viewer;
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
        case 70: //f
          this.toggleFavorite();
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
        default:
          console.log(e.which);
          break;
      }
    },
    onResize() {
      this.setImage();
    },
    setViewOptionsSelected: function (options) {
      var selected = [];
      if (options.showFaceBox) {
        selected.push("SHOW_FACE_BOXES");
      }
      if (options.showFaceList) {
        selected.push("SHOW_FACE_LIST");
      }
      if (options.showFilmStripe) {
        selected.push("SHOW_FILM_STRIPE");
      }
      if (options.showObjects) {
        selected.push("SHOW_OBJECTS");
      }

      this.settingsMenu.viewer.selected = selected;
    },
    onViewOptionsChange: function () {
      var options = {
        showFaceBox: this.settingsMenu.viewer.selected.includes(
          "SHOW_FACE_BOXES"
        ),
        showFaceList: this.settingsMenu.viewer.selected.includes(
          "SHOW_FACE_LIST"
        ),
        showFilmStripe: this.settingsMenu.viewer.selected.includes(
          "SHOW_FILM_STRIPE"
        ),
        showObjects: this.settingsMenu.viewer.selected.includes("SHOW_OBJECTS"),
      };

      this.$store.dispatch("media/setViewerOptions", options);
    },
    onClickFaceAction: function (action) {
      switch (action) {
        case "APPROVE_ALL":
          this.approveAll();
          break;
        case "UNASIGN_PREDICTED":
          this.unassignPredicted();
          break;
        case "DELETED_UNASSINGNED":
          this.deleteUnassigned();
          break;
      }
    },
    onActionClick: function (action) {
      switch (action) {
        case "RECYCLE":
          this.recycle();
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
    recycle() {
      this.$store.dispatch("media/recycle", [this.media.id]);
      this.navigate(1);
    },
    toggleInfo: function () {
      this.showInfoPage = !this.showInfoPage;
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

