<template>
  <v-app-bar app dense clipped-left color="indigo darken-4">
    <AppBarNavMenu></AppBarNavMenu>
    <v-menu left offset-y bottom>
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

    <v-menu
      left
      bottom
      offset-y
      class="d-sm-none-and-down"
      v-show="albumActions.length > 0"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-btn
          v-if="userActions.media.edit"
          v-bind="attrs"
          v-on="on"
          color="white"
          icon
          class="mr-1 d-none d-md-block"
        >
          <v-icon>mdi-image-album</v-icon>
        </v-btn>
      </template>

      <v-list dense>
        <v-list-item
          v-for="action in albumActions"
          :key="action.text"
          @click="onClickAlbumAction(action.action)"
        >
          <v-list-item-icon>
            <v-icon v-text="action.icon"></v-icon>
          </v-list-item-icon>
          <v-list-item-title> {{ action.text }}</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>

    <v-btn
      v-if="userActions.media.edit"
      @click="toggleEditMode"
      color="white"
      icon
      class="mr-0 ml-0"
    >
      <v-icon>mdi-check-box-multiple-outline</v-icon>
    </v-btn>
    <v-menu left offset-y bottom v-if="isEditMode">
      <template v-slot:activator="{ on, attrs }">
        <a v-bind="attrs" v-on="on">
          <h4 class="white--text ml-4">{{ selectedCount }} selected</h4></a
        >
      </template>
      <v-list dense>
        <v-list-item
          v-for="action in mediaActions"
          v-show="selectedCount > 0"
          :key="action.text"
          @click="onClickAction(action.action)"
        >
          <v-list-item-icon>
            <v-icon v-text="action.icon"></v-icon>
          </v-list-item-icon>
          <v-list-item-title> {{ action.text }}</v-list-item-title>
        </v-list-item>
        <v-divider></v-divider>
        <v-list-item @click="selectAll">
          <v-list-item-icon>
            <v-icon>mdi-select-all</v-icon>
          </v-list-item-icon>

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

    <h4 class="white--text mr-4" v-if="totalLoaded > 0 && !isEditMode">
      {{ totalLoaded }}
    </h4>

    <v-icon
      v-if="userActions.media.upload"
      color="white"
      class="mr-2"
      @click="openUpload"
    >
      mdi-cloud-upload-outline
    </v-icon>

    <NotificationMenu></NotificationMenu>

    <me-menu></me-menu>

    <Upload :show="showUpload"></Upload>
    <MoveMediaDialog
      :show="showMove"
      @close="showMove = false"
    ></MoveMediaDialog>

    <AddToAlbumDialog
      :show="showAddToAlbum"
      :context="addAlbumType"
      @close="showAddToAlbum = false"
    ></AddToAlbumDialog>

    <edit-media-dialog
      :show="showEditDialog"
      @close="showEditDialog = false"
    ></edit-media-dialog>

    <GlobalEvents
      v-if="userActions.media.edit"
      @keydown="keyPressed"
      :filter="(event, handler, eventName) => event.target.tagName !== 'INPUT'"
    ></GlobalEvents>
  </v-app-bar>
</template>

<script>
import Upload from "./Upload";
import MoveMediaDialog from "./MoveMediaDialog";
import AppBarNavMenu from "../AppBarNavMenu";
import NotificationMenu from "../Common/NotificationMenu";
import AddToAlbumDialog from "../Album/AddToAlbumDialog";
import EditMediaDialog from "./EditMediaDialog";
import GlobalEvents from "vue-global-events";
import MeMenu from "../MeMenu";
import { mapGetters, mapState } from "vuex";
import { getAuthorized, resources } from "../../services/resources";

export default {
  name: "App",
  components: {
    Upload,
    AppBarNavMenu,
    MoveMediaDialog,
    NotificationMenu,
    AddToAlbumDialog,
    EditMediaDialog,
    GlobalEvents,
    MeMenu,
  },

  data: () => ({
    dialog: true,
    showMove: false,
    showAddToAlbum: false,
    showEditDialog: false,
    addAlbumType: null,
    sizes: [
      {
        text: "Square XS",
        code: "SQ_XS",
      },
      { text: "Square S", code: "SQ_S" },
      { text: "Small", code: "S" },
      { text: "Medium", code: "M" },
      { text: "Large", code: "L" },
      { text: "Details", code: "D" },
      { text: "Table", code: "T" },
    ],
    showUpload: false,
  }),
  computed: {
    ...mapGetters("user", ["userActions"]),
    ...mapState("user", ["me"]),
    mediaActions: function () {
      return getAuthorized(resources.mediaActions, this.me.permissions);
    },
    albumActions: function () {
      var actions = [];
      if (this.$store.state.media.filter.folder != null) {
        actions.push({
          text: "Add Folder to album",
          action: "ADD_FOLDER",
          icon: "mdi-folder-outline",
        });
      }
      if (this.$store.getters["media/filterDescriptions"].length > 0) {
        actions.push({
          text: "Create from filters",
          action: "CREATE_FROM_QUERY",
          icon: "mdi-playlist-plus",
        });
      }

      return actions;
    },
    isEditMode: function () {
      return this.$store.state.media.isEditMode;
    },
    totalLoaded: function () {
      return this.$store.state.media.totalLoaded;
    },
    totalCount: function () {
      return this.$store.state.media.totalCount;
    },
    selectedCount: function () {
      return this.$store.state.media.selectedIndexes.length;
    },
    loading: function () {
      return this.$store.state.media.listLoading;
    },
  },
  methods: {
    setSize: function (code) {
      this.$store.dispatch("media/setThumbnailSize", code);
    },
    openUpload: function () {
      this.$store.dispatch("media/toggleUploadDialog", true);
    },
    toggleEditMode: function () {
      this.$store.dispatch("media/toggleEditMode");
    },
    selectAll: function () {
      this.$store.dispatch("media/selectAll");
    },
    clearSelected: function () {
      this.$store.dispatch("media/clearSelected");
    },
    onMediaAction: function (action) {
      console.log(action);
      if (action.action === "ADD_TO_ALBUM") {
        this.addAlbumType = "ID";
        this.showAddToAlbum = true;
      } else if (action.action === "MOVE") {
        this.showMove = true;
      }
    },
    onClickAction: function (action) {
      switch (action) {
        case "MOVE":
          this.showMove = true;
          break;
        case "ADD_TO_ALBUM":
          this.addAlbumType = "IDS";
          this.showAddToAlbum = true;
          break;
        case "EDIT":
          this.showEditDialog = true;
          break;
        case "RECYCLE":
          this.$store.dispatch("media/recycle");
          break;
        case "DELETE":
          this.$store.dispatch("media/delete");
          break;
        case "SHARE":
          this.$store.dispatch("media/shareSelected");
          break;
      }
    },
    onClickAlbumAction: function (action) {
      switch (action) {
        case "ADD_FOLDER":
          this.addAlbumType = "FOLDER";
          this.showAddToAlbum = true;
          break;
        case "CREATE_FROM_QUERY":
          this.addAlbumType = "QUERY";
          this.showAddToAlbum = true;
          break;
      }
    },
    keyPressed: function (e) {
      if (!this.userActions.media.edit) {
        return;
      }
      switch (e.which) {
        case 69: //e
          this.$store.dispatch("media/toggleEditMode");
          break;
        case 65: //a
          if (this.$store.state.media.isEditMode) {
            if (this.$store.state.media.selectedIndexes.length === 0) {
              this.selectAll();
            } else {
              this.clearSelected();
            }
          }
          break;
        case 88: //x
          this.$store.dispatch("person/buildModel");
          break;
        case 68: //d
          this.$store.dispatch("media/toggleLoadThumbnailData");
          break;
      }
    },
  },
};
</script>
<style >
</style>

