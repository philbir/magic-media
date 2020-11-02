<template>
  <div v-resize="onResize">
    <div v-if="loading">Loading...</div>
    <div v-else class="media-wrapper" @mousemove="onMouseMove">
      <Keypress key-event="keyup" @success="keyPressed" />

      <div class="media-nav">
        <v-row>
          <v-col class="ma-4">
            <v-icon large color="grey darken-1" @click="handlePrevious">
              mdi-chevron-left-circle
            </v-icon>
          </v-col>
          <v-spacer></v-spacer>
          <v-col justify="end" align="right" class="ma-3">
            <v-icon large color="grey darken-1" @click="handleNext">
              mdi-chevron-right-circle
            </v-icon>
          </v-col>
        </v-row>
      </div>
      <div class="head">
        <v-row>
          <v-col class="ml-2">
            <v-icon @click="handleHome" color="white"> mdi-home </v-icon>
            {{ media.filename }}
          </v-col>
          <v-spacer></v-spacer>
          <v-col class="mr-4" align="right">
            <v-icon color="white"> mdi-cog-outline </v-icon>
          </v-col>
        </v-row>
      </div>
      <div class="foot">
        {{ geoLocation }}
      </div>
      <img :src="imageSrc" @load="onImgLoaded" ref="img" />
      <div v-show="image.loaded">
        <template v-for="face in media.faces">
          <FaceBox :key="face.id" :face="face" :image="image"></FaceBox>
        </template>
      </div>
    </div>
    <div class="filmstripe" v-show="showStripe">
      <FilmStripe></FilmStripe>
    </div>
  </div>
</template>

<script>
import FaceBox from "../components/FaceBox.vue";
import FilmStripe from "../components/FileStripe.vue";
//import trottle from "lodash";

export default {
  data() {
    return {
      image: {
        loaded: false,
        width: 0,
        naturalWidth: 0,
        offsetLeft: 0,
        offsetTop: 0,
      },
      showStripe: false,
      keyboardKeys: [
        {
          keyCode: 37, // Left
          preventDefault: false,
        },
        {
          keyCode: 39, // Right
          preventDefault: false,
        },
        {
          keyCode: 27, // ESC
          preventDefault: false,
        },
      ],
      mediaId: this.$route.params.id,
      windowWidth: window.innerWidth,
    };
  },
  components: { FaceBox, Keypress: () => import("vue-keypress"), FilmStripe },
  created() {
    this.$store.dispatch("loadMediaDetails", this.$route.params.id);
  },
  beforeRouteUpdate(to, from, next) {
    console.log(to);
    this.image.loaded = false;

    this.$store.dispatch("loadMediaDetails", to.params.id).then(() => {
      next();
    });
  },
  beforeRouteLeave(to, from, next) {
    this.image.loaded = false;
    next();
  },
  computed: {
    thumbnail: function () {
      if (this.$store) {
        const existing = this.$store.mediaList.filter(
          (x) => x.id == this.$route.params.id
        );
        if (existing.length > 0) {
          return existing.thumbnail.dataUrl;
        }
      }

      return null;
    },
    imageSrc: function () {
      if (this.media.id === this.$route.params.id) {
        return "/api/media/webimage/" + this.media.id;
      }
      return null;
    },
    loading: function () {
      //return this.image.loading;
      return this.$store.state.currentMedia === null;
    },
    media: function () {
      return this.$store.state.currentMedia;
    },
    geoLocation: function () {
      if (
        this.$store.state.currentMedia.geoLocation &&
        this.$store.state.currentMedia.geoLocation.address
      ) {
        return this.$store.state.currentMedia.geoLocation.address.name;
      }
      return null;
    },
    box: function () {
      const media = this.$store.state.currentMedia;

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
      var next = this.$store.getters.nextMedia(step);
      if (next) {
        this.$router.replace({ name: "media", params: { id: next.id } });
      } else {
        this.$router.push("/");
      }
    },
    handleHome: function () {
      this.$router.push("/");
    },
    onMouseMove: function (e) {
      this.showStripe = e.clientY > 300;
    },
    keyPressed: function (e) {
      console.log(e.event.keyCode);
      switch (e.event.keyCode) {
        case 37:
          this.navigate(-1);
          break;
        case 39:
          this.navigate(1);
          break;
        case 27:
          this.$router.push("/");
          break;
      }
      console.log(e.event.key, e.event.keyCode);
    },
    onResize() {
      this.setImage();
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
  width: 100%;
  top: 44vh;
  z-index: 1000;
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
  z-index: 10;
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
}
</style>

