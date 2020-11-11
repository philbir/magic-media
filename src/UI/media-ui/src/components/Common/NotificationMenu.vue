<template>
  <v-menu offset-y>
    <template v-slot:activator="{ on, attrs }">
      <v-badge
        :value="activeNotifications.length > 0"
        :content="activeNotifications.length"
        color="red"
        overlap
      >
        <v-icon color="white" v-bind="attrs" v-on="on" class="mr-2 ml-2">
          mdi-bell-outline
        </v-icon>
      </v-badge>
    </template>

    <v-list width="360">
      <template v-for="ntf in activeNotifications">
        <v-list-item :key="ntf.id">
          <v-list-item-content>
            <v-alert dense :type="ntf.type.toLowerCase()">
              {{ ntf.text }}</v-alert
            >
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
    activeNotifications: function () {
      return this.$store.state.snackbar.notifications.filter((x) => x.active);
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
