<template>
  <AppPreLoader>
    <Upload :show="showUpload"></Upload>
    <v-app>
      <router-view name="appbar"></router-view>
      <v-navigation-drawer clipped v-if="!isFullscreen" app>
        <router-view name="left"></router-view>
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
import MediaViewer from "./components/MediaViewer";
import VuePageTransition from "vue-page-transition";
import Vue from "vue";

Vue.use(VuePageTransition);

export default {
  name: "App",
  components: { AppPreLoader, Upload, MediaViewer },
  created() {
    const self = this;
    this.$socket.on("mediaOperationCompleted", (data) => {
      self.$store.dispatch("snackbar/moveMediaCompleted", data);
    });
    this.$socket.on("mediaOperationRequestCompleted", (data) => {
      self.$store.dispatch("snackbar/moveMediaRequestCompleted", data);
      this.$store.dispatch("media/getFolderTree");
      this.$magic.snack("Move completed! " + data.successCount, "SUCCESS");
    });
  },
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
    selectAll: function () {
      this.$store.dispatch("media/selectAll");
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

::-webkit-scrollbar {
  width: 2px;
  height: 2px;
}
::-webkit-scrollbar-button {
  width: 0px;
  height: 0px;
}
::-webkit-scrollbar-thumb {
  background: #e1e1e1;
  border: 0px none #ffffff;
  border-radius: 50px;
}
::-webkit-scrollbar-thumb:hover {
  background: #ffffff;
}
::-webkit-scrollbar-thumb:active {
  background: #000000;
}
::-webkit-scrollbar-track {
  background: #666666;
  border: 0px none #ffffff;
  border-radius: 50px;
}
::-webkit-scrollbar-track:hover {
  background: #666666;
}
::-webkit-scrollbar-track:active {
  background: #333333;
}
::-webkit-scrollbar-corner {
  background: transparent;
}
</style>

