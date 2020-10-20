<template>
  <div ref="main">
    <div v-if="loading">Loading...</div>
    <div v-else>
      <img
        class="center-image"
        :src="'http://localhost:5000/media/webimage/' + media.id"
      />
    </div>
  </div>
</template>

<script>
import QUERY_GETBYID from "../graphql/GetMediaDetails.gql";

export default {
  data() {
    return {
      mediaId: this.$route.params.id,
      windowWidth: window.innerWidth,
      media: {},
      loading: true,
    };
  },
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
      console.log(media);
      console.log(this.$refs.main.clientHeight);
      this.media = media;
      this.loading = false;
    },
  },
};
</script>

<style scoped>
</style>

