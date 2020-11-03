<template>
  <AppPreLoader>
    <Upload :show="showUpload"></Upload>
    <v-app>
      <v-app-bar
        app
        dense
        clipped-left
        v-if="!isFullscreen"
        color="indigo darken-4"
      >
        <v-app-bar-nav-icon color="white"></v-app-bar-nav-icon>
        <v-menu left bottom>
          <template v-slot:activator="{ on, attrs }">
            <v-btn color="white" icon v-bind="attrs" v-on="on">
              <v-icon>mdi-resize</v-icon>
            </v-btn>
          </template>

          <v-list dense>
            <v-list-item
              v-for="size in sizes"
              :key="size.code"
              @click="setSize(size.code)"
            >
              <v-list-item-title> {{ size.text }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <v-spacer></v-spacer>
        <v-icon color="white" class="mr-2" @click="openUpload">
          mdi-cloud-upload-outline
        </v-icon>

        <v-icon color="white"> mdi-cog </v-icon>
      </v-app-bar>

      <v-navigation-drawer clipped v-if="!isFullscreen" app>
        <FolderTree></FolderTree>
      </v-navigation-drawer>

      <v-main :class="{ fullscreen: isFullscreen }">
        <vue-page-transition>
          <router-view />
        </vue-page-transition>
      </v-main>

      <v-snackbar
        v-for="(snack, i) in snacks"
        :key="'snack' + i"
        :value="snack.show"
        rounded
        app
        bottom
        :color="snack.color"
      >
        <v-icon> {{ snack.icon }}</v-icon>
        {{ snack.text }}

        <template v-slot:action="">
          <v-icon @click="snack.show = false" color="white"> mdi-close </v-icon>
        </template>
      </v-snackbar>
    </v-app>
  </AppPreLoader>
</template>

<script>
import AppPreLoader from "./components/AppPreLoader";
import Upload from "./components/Upload";
import FolderTree from "./components/FolderTree";
import VuePageTransition from "vue-page-transition";
import Vue from "vue";

Vue.use(VuePageTransition);

export default {
  name: "App",
  components: { AppPreLoader, FolderTree, Upload },

  data: () => ({
    sizes: [
      {
        text: "Square XS",
        code: "SQ_XS",
      },
      { text: "Square S", code: "SQ_S" },
      { text: "Small", code: "S" },
      { text: "Medium", code: "M" },
      { text: "Large", code: "L" },
    ],
    showUpload: false,
  }),
  computed: {
    snacks: function () {
      return this.$store.state.snackbar.snacks.map((snack) => {
        switch (snack.type) {
          case "INFO":
            snack.icon = "mdi-information-outline";
            snack.color = "blue";
            break;
          case "WARN":
            snack.icon = "mdi-alert-outline";
            snack.color = "yellow";
            break;
          case "ERROR":
            snack.icon = "mdi-nuke";
            snack.color = "red";
            break;
          case "SUCCESS":
            snack.icon = "mdi-check-circle-outline";
            snack.color = "green";
            break;
        }

        return snack;
      });
    },
    isFullscreen: function () {
      if (this.$route.meta.fullscreen) {
        return true;
      } else {
        return false;
      }
    },
  },
  methods: {
    setSize: function (code) {
      this.$store.dispatch("media/setThumbnailSize", code);
    },
    openUpload: function () {
      this.$store.dispatch("media/toggleUploadDialog", true);
    },
  },
};
</script>
<style >
html {
  overflow-x: hidden !important;
  overflow-y: hidden !important;
}

main {
  height: 90vh;
  overflow-y: auto;
  overflow-x: hidden;
}

.fullscreen {
  height: auto;
  overflow-y: hidden;
}
</style>

