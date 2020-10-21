<template>
  <div>
    <div v-if="loading">Loading...</div>
    <div v-else class="media-wrapper">
      <img
        :src="'http://localhost:5000/media/webimage/' + media.id"
        ref="img"
        :style="{
          'margin-left': box.left + 'px',
          'margin-top': box.top + 'px',
          height: box.height + 'px',
          width: box.width + 'px',
        }"
      />

      <template v-for="face in media.faces">
        <FaceBox :key="face.id" :face="face"></FaceBox>
      </template>
    </div>
  </div>
</template>

<script>
import QUERY_GETBYID from "../graphql/GetMediaDetails.gql";
import FaceBox from "../components/FaceBox.vue";

export default {
  data() {
    return {
      mediaId: this.$route.params.id,
      windowWidth: window.innerWidth,
      media: {},
      box: {},
      loading: true,
    };
  },
  components: { FaceBox },
  mounted() {
    this.$apollo
      .query({
        query: QUERY_GETBYID,
        variables: {
          id: this.$route.params.id,
        },
      })
      .then((res) => {
        console.log(res);
        this.setMedia(res.data.mediaById);
      });
  },
  methods: {
    setMedia(media) {
      this.media = media;
      this.loading = false;
      this.box = this.getBox(media);
    },
    getBox(media) {
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

