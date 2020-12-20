<template>
  <v-menu offset-y v-if="me">
    <template v-slot:activator="{ on, attrs }">
      <v-btn
        color="white"
        icon
        v-bind="attrs"
        v-on="on"
        @contextmenu="debug($event)"
      >
        <v-icon>mdi-account</v-icon>
      </v-btn>
    </template>

    <v-card height="150" width="300">
      <v-list-item two-line>
        <v-list-item-content>
          <v-list-item-title class="headline">
            {{ me.name }}
          </v-list-item-title>
          <v-list-item-subtitle>{{ me.email }}</v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>

      <v-card-title> </v-card-title>

      <v-divider></v-divider>

      <v-card-actions>
        <v-icon class="ml-2">mdi-tune-variant</v-icon>
        <v-icon class="ml-4">mdi-help-circle-outline</v-icon>

        <v-spacer></v-spacer>
        <v-icon class="mr-0">mdi-logout</v-icon>
      </v-card-actions>
    </v-card>
  </v-menu>
</template>

<script>
import { mapState } from "vuex";
export default {
  data: () => ({
    menuItems: [
      {
        text: "Logout",
        icon: "mdi-logout",
        action: "LOGOUT",
      },
    ],
  }),
  computed: {
    ...mapState("user", ["me"]),
  },
  methods: {
    debug: function (e) {
      e.preventDefault();
      console.log(this.$vuetify);
    },
  },
};
</script>

<style>
</style>