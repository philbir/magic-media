<template>
  <div class="ma-8">
    <h2>Settings</h2>

    <v-btn @click="buildModel"> Build Face model </v-btn>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  methods: {
    ...mapActions("person", ["buildModel"]),
  },
};
</script>

<style lang="scss" scoped>
</style>>
