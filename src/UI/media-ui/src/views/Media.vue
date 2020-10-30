<template>
  <div>
    <div v-if="loading">Loading...</div>
    <div v-else class="media-wrapper">
      <img
        :src="'/api/media/webimage/' + mediaId"
        @load="onImgLoaded"
        ref="img"
        :style="{
          'margin-left': box.left + 'px',
          'margin-top': box.top + 'px',
          height: box.height + 'px',
          width: box.width + 'px',
        }"
      />

      <template v-show="imagedLoaded" v-for="face in media.faces">
        <FaceBox :key="face.id" :face="face" :image="image"></FaceBox>
      </template>
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
          this.image = {
            width: this.$refs.img.width,
            naturalWidth: this.$refs.img.naturalWidth,
            offsetLeft: this.$refs.img.offsetLeft,
            offsetTop: this.$refs.img.offsetTop,
            loaded: true,
          };
          console.log(this.image);
        }, 250);
      });
    },
  },
};
</script>

<style scoped>
.media-wrapper {
  background-color: #1b1b1c;
  height: 100vh;
  z-index: 1;
  overflow: hidden;
}
</style>

