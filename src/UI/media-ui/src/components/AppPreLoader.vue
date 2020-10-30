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
    this.$store.dispatch("getAllPersons").then((e) => {
      console.log(e);
      this.preloaded = true;
    });
  },
};
</script>

<style scoped>
</style>