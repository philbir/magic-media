<template>
  <v-app-bar app dense clipped-left color="indigo darken-4">
    <AppBarNavMenu></AppBarNavMenu>

    <v-spacer></v-spacer>
    <v-progress-circular
      indeterminate
      :size="22"
      :width="2"
      color="white"
      class="mr-4 ml-1"
      v-show="loading"
    ></v-progress-circular>
  </v-app-bar>
</template>

<script>
import AppBarNavMenu from "../AppBarNavMenu.vue";

export default {
  components: { AppBarNavMenu },

  data: () => ({}),
  computed: {
    loading: function () {
      return this.$store.state.map.loading;
    },
  },
  methods: {},
};
</script>
<style >
</style>

