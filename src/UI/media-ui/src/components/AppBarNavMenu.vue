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
import { mapState } from "vuex";
import { resources, getAuthorized } from "../services/resources";
export default {
  data: () => ({}),
  computed: {
    ...mapState("user", ["me"]),
    navMenuItems: function () {
      return getAuthorized(resources.navMenu, this.me.permissions);
    },
  },
};
</script>

<style>
</style>