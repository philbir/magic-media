<template>
  <div v-if="ready">
    <slot></slot>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  name: "AppPreLoader",
  data: () => ({
    preloaded: false,
  }),
  computed: {
    ...mapState("user", ["me"]),
    ready: function () {
      return this.preloaded && this.me != null;
    },
  },
  created() {
    this.$store.dispatch("person/getAll").then(() => {
      this.preloaded = true;
    });
    this.$store.dispatch("person/getAllGroups").then(() => {
      this.preloaded = true;
    });
    this.$store.dispatch("user/getAll").then(() => {
      this.preloaded = true;
    });
    this.$store.dispatch("user/getMe");
  },
};
</script>

<style scoped>
</style>
