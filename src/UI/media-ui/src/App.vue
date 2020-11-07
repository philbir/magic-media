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

        <v-switch
          dense
          @change="toggleEditMode"
          color="info"
          value="edit"
          class="mt-4 ml-2"
        >
          <template v-slot:label>
            <span class="white--text">{{ editModeText }}</span>
          </template>
        </v-switch>

        <v-menu left bottom v-if="editModeText == 'Edit'">
          <template v-slot:activator="{ on, attrs }">
            <a v-bind="attrs" v-on="on">
              <h4 class="white--text ml-4">{{ selectedCount }} selected</h4></a
            >
          </template>
          <v-list dense>
            <v-list-item v-for="action in mediaActions" :key="action.text">
              <v-list-item-title> {{ action.text }}</v-list-item-title>
            </v-list-item>
            <v-divider></v-divider>
            <v-list-item>
              <v-list-item-title>Select all</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-spacer></v-spacer>
        <v-progress-circular
          indeterminate
          :size="22"
          :width="2"
          color="white"
          class="mr-4 ml-1"
          v-show="loading"
        ></v-progress-circular>
        <h4 class="white--text mr-4">{{ mediaCount }}</h4>

        <v-icon color="white" class="mr-2" @click="openUpload">
          mdi-cloud-upload-outline
        </v-icon>

        <v-icon color="white"> mdi-cog </v-icon>
      </v-app-bar>

      <v-navigation-drawer clipped v-if="!isFullscreen" app>
        <MediaFilter></MediaFilter>
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
    <v-dialog
      v-model="mediaViewerOpen"
      fullscreen
      transition="scale-transition"
    >
      <MediaViewer v-if="mediaViewerOpen"></MediaViewer>
    </v-dialog>
  </AppPreLoader>
</template>

<script>
import AppPreLoader from "./components/AppPreLoader";
import Upload from "./components/Upload";
import MediaFilter from "./components/MediaFilter";
import MediaViewer from "./components/MediaViewer";
import VuePageTransition from "vue-page-transition";
import Vue from "vue";

Vue.use(VuePageTransition);

export default {
  name: "App",
  components: { AppPreLoader, Upload, MediaFilter, MediaViewer },

  data: () => ({
    dialog: true,
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
    mediaActions: function () {
      return [
        { text: "Add to album" },
        { text: "Move" },
        { text: "Edit" },
        { text: "Delete" },
      ];
    },
    editModeText: function () {
      return this.$store.state.media.isEditMode ? "Edit" : "View";
    },
    mediaViewerOpen: function () {
      return this.$store.state.media.currentMediaId != null;
    },
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
    mediaCount: function () {
      return this.$store.state.media.list.length;
    },
    selectedCount: function () {
      return this.$store.state.media.selectedIndexes.length;
    },
    loading: function () {
      return this.$store.state.media.listLoading;
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
    toggleEditMode: function (value) {
      this.$store.dispatch("media/toggleEditMode", value === "edit");
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
  overflow-y: hidden;
  overflow-x: hidden;
}

.v-dialog--fullscreen {
  overflow: hidden !important;
  overflow-x: hidden !important;
}
</style>

