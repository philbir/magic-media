<template>
  <div class="container">
    <img
      class="thumb"
      :class="{ selected: media.isSelected }"
      v-for="media in medias"
      :key="media.id"
      :src="media.thumbnail.dataUrl"
      :ref="'img' + media.id"
      @click="onMediaClick(media.id)"
    />
  </div>
</template>

<script>
//import * as easings from "vuetify/es5/services/goto/easing-patterns";

export default {
  data() {
    return {};
  },
  computed: {
    medias: function () {
      let self = this;
      return this.$store.state.mediaList.map((item) => {
        if (
          self.$store.state.currentMedia &&
          self.$store.state.currentMedia.id === item.id
        ) {
          item.isSelected = true;
        } else item.isSelected = false;
        return item;
      });
    },
  },
  methods: {
    onMediaClick: function (id) {
      const r = this.$refs["img" + id][0];
      this.$vuetify.goTo(r, {});
      this.$router.replace({ name: "media", params: { id } });
    },
  },
};
</script>


<style scoped>
.container {
  display: block;
  padding: 0;
  overflow-x: hidden;
  overflow-y: hidden;
  white-space: nowrap;
  overflow-x: scroll;
  transition: all 0.2s ease-in-out;
  transition-property: all;
  transition-duration: 0.2s;
  transition-timing-function: ease-in-out;
  transition-delay: 0s;
}

.container::-webkit-scrollbar {
  display: block;
}

.thumb {
  height: 65px;
  width: 65px;
  object-fit: cover;
  border-radius: 10px;
  margin-left: 10px;
  opacity: 0.75;
}

.thumb:hover {
  opacity: 1;
}

.selected {
  border: 2px solid white;
}
</style>