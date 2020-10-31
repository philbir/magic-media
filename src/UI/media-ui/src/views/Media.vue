<template>
  <div v-resize="onResize">
    <div v-if="loading">Loading...</div>
    <div v-else class="media-wrapper">
      <div class="media-nav">
        <v-row>
          <v-col class="ma-4">
            <v-icon large color="grey darken-1" @click="handlePrevious">
              mdi-chevron-left-circle
            </v-icon>
          </v-col>
          <v-spacer></v-spacer>
          <v-col justify="end" align="right" class="ma-3" @click="handleNext">
            <v-icon large color="grey darken-1">
              mdi-chevron-right-circle
            </v-icon>
          </v-col>
        </v-row>
      </div>
      <img :src="imageSrc" @load="onImgLoaded" ref="img" />
      <div v-show="image.loaded">
        <template v-for="face in media.faces">
          <FaceBox :key="face.id" :face="face" :image="image"></FaceBox>
        </template>
      </div>
    </div>
  </div>
</template>

<script>
import FaceBox from "../components/FaceBox.vue";

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
      mediaId: this.$route.params.id,
      windowWidth: window.innerWidth,
    };
  },
  components: { FaceBox },
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
      var next = this.$store.getters.nextMedia(-1);
      if (next) {
        this.$router.replace({ name: "media", params: { id: next.id } });
      } else {
        this.$router.push("/");
      }
    },
    handleNext: function () {
      var next = this.$store.getters.nextMedia(+1);
      if (next) {
        this.$router.replace({ name: "media", params: { id: next.id } });
      } else {
        this.$router.push("/");
      }
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
</style>

