<template>
  <div v-if="ready">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: "AppPreLoader",
  data: () => ({
    preloaded: false,
  }),
  computed: {
    ready: function () {
      return this.preloaded;
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
  },
};
</script>

<style scoped>
</style>
