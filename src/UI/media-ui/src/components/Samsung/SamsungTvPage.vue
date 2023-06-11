<template>
  <div>
    <v-progress-linear v-if="loading" indeterminate color="blue" top />
    <div
      style="overflow-x: auto; height: 92vh"
      class="d-flex flex-wrap flex-row justify-start"
    >
      <div class="ma-2" v-for="media in medias" :key="media.contentId">
        <media-item :media="media" />
      </div>
    </div>
  </div>
</template>

<script>
//import { mapActions, mapState } from "vuex";
import MediaItem from "./MediaItem";

export default {
  components: {
    MediaItem
  },
  data() {
    return {};
  },
  created() {
    this.$store.dispatch("samsung/getDevices");
  },
  computed: {
    medias: function() {
      return this.$store.state.samsung.media.list;
    },
    loading: function() {
      return this.$store.state.samsung.media.loading;
    }
  },
  methods: {}
};
</script>

<style scoped>
.media-item {
  position: relative;
  height: 140px;
}

.media-footer {
  bottom: 0px;
  width: 100%;
  height: 40px;
  background-color: #504f4f;
  opacity: 0.8;
  position: absolute;
  color: #fff;
  font-weight: bolder;
  padding: 6px;
  font-size: 1em;
  border-radius: 6px;
}
</style>
