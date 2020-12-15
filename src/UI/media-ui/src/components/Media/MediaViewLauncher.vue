<template>
  <div class="ma-10 grey--text">
    <h3>
      image viewer is launching... 🚀 <small>{{ mediaId }}</small>
    </h3>
    <br />
    <router-link :to="{ name: 'Home' }">🏠</router-link>
  </div>
</template>

<script>
import { mapActions } from "vuex";
export default {
  created() {
    this.show(this.mediaId);
  },
  data() {
    return {};
  },
  computed: {
    mediaId: function () {
      return this.$route.params.id.replaceAll("-", "");
    },
  },
  methods: {
    ...mapActions("media", ["show"]),
  },
};
</script>

<style>
</style>