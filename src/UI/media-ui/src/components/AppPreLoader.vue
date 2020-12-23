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
    ...mapState("user", ["me", "error"]),
    ready: function () {
      return this.preloaded && this.me != null;
    },
  },
  watch: {
    error: function (newValue) {
      if (newValue) {
        this.$router.push({ name: "Error" });
      }
    },
  },
  created() {
    if (this.error) return;

    this.$store.dispatch("user/getMe");

    this.$store.dispatch("person/getAll").then(() => {
      this.preloaded = true;
    });
    this.$store.dispatch("person/getAllGroups").then(() => {
      this.preloaded = true;
    });
    this.$store.dispatch("user/getAll").then(() => {
      this.preloaded = true;
    });
  },
};
</script>

<style scoped>
</style>
