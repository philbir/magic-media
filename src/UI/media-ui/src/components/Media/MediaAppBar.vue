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
          class="mr-4"
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

    <v-switch
      v-if="userActions.media.edit"
      dense
      @change="toggleEditMode"
      color="info"
      value="edit"
      class="mt-4 d-none d-md-block"
    >
      <template v-slot:label>
        <span class="white--text">{{ editModeText }}</span>
      </template>
    </v-switch>

    <v-menu
      left
      offset-y
      class="d-sm-none-and-down"
      bottom
      v-if="editModeText == 'Edit'"
    >
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
    <h4 class="white--text mr-4" v-if="totalLoaded > 0">
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
import { mapGetters } from "vuex";

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
    ],
    showUpload: false,
  }),
  computed: {
    ...mapGetters("user", ["userActions"]),
    mediaActions: function () {
      return [
        { text: "Add to album", action: "ADD_TO_ALBUM", icon: "mdi-plus" },
        { text: "Move", action: "MOVE", icon: "mdi-file-move-outline" },
        { text: "Edit", action: "EDIT", icon: "mdi-pencil" },
        { text: "Recycle", action: "RECYCLE", icon: "mdi-recycle" },
        { text: "Delete", action: "DELETE", icon: "mdi-delete" },
      ];
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
    editModeText: function () {
      return this.$store.state.media.isEditMode ? "Edit" : "View";
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
    toggleEditMode: function (value) {
      this.$store.dispatch("media/toggleEditMode", value === "edit");
    },
    selectAll: function () {
      this.$store.dispatch("media/selectAll");
    },
    clearSelected: function () {
      this.$store.dispatch("media/clearSelected");
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
        case 77: //m
          this.$store.dispatch("person/buildModel");
          break;
        default:
          console.log("KEYPress MediaBar", e.which);
      }
    },
  },
};
</script>
<style >
</style>

