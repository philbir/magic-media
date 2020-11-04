<template>
  <div v-resize="onResize">
    <div v-if="loading">Loading...</div>

    <v-carousel
      v-else
      height="100%"
      hide-delimiter-background
      show-arrows-on-hover
      hide-delimiters
      @change="changed"
    >
      <v-carousel-item v-for="(m, i) in items" :key="i">
        <div class="media-wrapper">
          <img
            :src="'/api/media/webimage/' + mediaId"
            @load="onImgLoaded"
            ref="img"
          />
          <template v-show="imagedLoaded" v-for="face in media.faces">
            <FaceBox :key="face.id" :face="face" :image="image"></FaceBox>
          </template>
        </div>
      </v-carousel-item>
    </v-carousel>
  </div>
</template>

<script>
import FaceBox from "../components/FaceBox.vue";

export default {
  data() {
    return {
      items: [1, 2, 3, 4, 5, 6, 7, 8],
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
  computed: {
    loading: function () {
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
        }, 250);
      });
    },
    setImage() {
      this.image = {
        width: this.$refs.img.width,
        naturalWidth: this.$refs.img.naturalWidth,
        offsetLeft: this.$refs.img.offsetLeft - this.$refs.img.width / 2,
        offsetTop: this.$refs.img.offsetTop - this.$refs.img.height / 2,
        loaded: true,
      };
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
</style>

