<template>
  <v-menu offset-y>
    <template v-slot:activator="{ on, attrs }">
      <v-app-bar-nav-icon
        v-bind="attrs"
        v-on="on"
        color="white"
      ></v-app-bar-nav-icon>
    </template>

    <v-list dense nav width="200">
      <v-list-item
        v-for="item in navMenuItems"
        :key="item.text"
        :to="{ name: item.route }"
      >
        <v-list-item-icon>
          <v-icon>{{ item.icon }}</v-icon>
        </v-list-item-icon>

        <v-list-item-content>
          <v-list-item-title>{{ item.text }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </v-menu>
</template>

<script>
export default {
  data: () => ({}),
  computed: {
    navMenuItems: function () {
      return [
        {
          text: "Media",
          icon: "mdi-image",
          route: "Home",
        },
        {
          text: "Face",
          icon: "mdi-face-recognition",
          route: "Faces",
        },
        {
          text: "Persons",
          icon: "mdi-account-details",
          route: "Persons",
        },
        {
          text: "Albums",
          icon: "mdi-image-album",
          route: "Albums",
        },
        {
          text: "Map",
          icon: "mdi-map-search-outline",
          route: "Map",
        },
        {
          text: "Users",
          icon: "mdi-account",
          route: "Users",
        },
        {
          text: "Settings",
          icon: "mdi-tune-variant",
          route: "Settings",
        },
      ];
    },
  },
};
</script>

<style>
</style>