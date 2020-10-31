<template>
  <v-app>
    <AppPreLoader>
      <!--
    <v-app-bar app color="primary" style="z-index: 10" light>
      <h2 class="white--text">magic media</h2>

      <v-spacer></v-spacer>
    </v-app-bar>
    -->
      <v-main>
        <vue-page-transition>
          <router-view />
        </vue-page-transition>
      </v-main>
    </AppPreLoader>
  </v-app>
</template>

<script>
import AppPreLoader from "./components/AppPreLoader";
import VuePageTransition from "vue-page-transition";
import Vue from "vue";

Vue.use(VuePageTransition);

export default {
  name: "App",
  components: { AppPreLoader },

  data: () => ({
    //
  }),
};
</script>
