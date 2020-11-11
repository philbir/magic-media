<template>
  <v-menu offset-y>
    <template v-slot:activator="{ on, attrs }">
      <v-badge :value="count" :content="count" color="green" overlap>
        <v-icon
          color="white"
          v-bind="attrs"
          v-on="on"
          class="mr-2 ml-2"
          @click="openUpload"
        >
          mdi-bell-outline
        </v-icon>
      </v-badge>
    </template>

    <v-list two-line width="360">
      <template>
        <v-list-item>
          <v-list-item-icon>
            <v-icon>mdi-information-outline</v-icon>
          </v-list-item-icon>

          <v-list-item-content>
            <v-list-item-title>Media Moved</v-list-item-title>
            <v-list-item-subtitle>1 / 2</v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </template>
    </v-list>
  </v-menu>
</template>

<script>
export default {
  data: () => ({}),
  computed: {
    count: function () {
      return 10;
    },
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
          text: "Album",
          icon: "mdi-image-album",
          route: "Albums",
        },
        {
          text: "Map",
          icon: "mdi-map-search-outline",
          route: "Map",
        },
      ];
    },
  },
};
</script>

<style>
</style>