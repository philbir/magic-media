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
    <me-menu></me-menu>
  </v-app-bar>
</template>

<script>
import AppBarNavMenu from "./AppBarNavMenu";
import MeMenu from "./MeMenu";
export default {
  components: { AppBarNavMenu, MeMenu },

  data: () => ({}),
  computed: {
    loading: function() {
      return false;
    }
  },
  methods: {}
};
</script>
<style></style>
