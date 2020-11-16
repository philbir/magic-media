<template>
  <div v-if="preloaded">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: "AppPreLoader",
  data: () => ({
    preloaded: false,
  }),
  created() {
    this.$store.dispatch("person/getAll").then(() => {
      this.preloaded = true;
    });
    this.$store.dispatch("person/getAllGroups").then(() => {
      this.preloaded = true;
    });
  },
};
</script>

<style scoped>
</style>
