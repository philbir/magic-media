<template>
  <v-app-bar app dense clipped-left color="indigo darken-4">
    <AppBarNavMenu></AppBarNavMenu>
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
      class="mt-4 d-none d-md-block"
    >
      <template v-slot:label>
        <span class="white--text">{{ editModeText }}</span>
      </template>
    </v-switch>

    <v-menu
      left
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
          <v-list-item-title> {{ action.text }}</v-list-item-title>
        </v-list-item>
        <v-divider></v-divider>
        <v-list-item @click="selectAll">
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
      {{ totalLoaded }} / {{ totalCount }}
    </h4>

    <v-icon color="white" class="mr-2" @click="openUpload">
      mdi-cloud-upload-outline
    </v-icon>
    <NotificationMenu></NotificationMenu>

    <Upload :show="showUpload"></Upload>
    <MoveMediaDialog
      :show="showMove"
      @close="showMove = false"
    ></MoveMediaDialog>
  </v-app-bar>
</template>

<script>
import Upload from "./Upload";
import MoveMediaDialog from "./MoveMediaDialog";
import AppBarNavMenu from "./AppBarNavMenu";
import NotificationMenu from "./Common/NotificationMenu";

export default {
  name: "App",
  components: { Upload, AppBarNavMenu, MoveMediaDialog, NotificationMenu },

  data: () => ({
    dialog: true,
    showMove: false,
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
        { text: "Add to album", action: "ADD_TO_ALBUM" },
        { text: "Move", action: "MOVE" },
        { text: "Edit", action: "EDIT" },
        { text: "Delete", acion: "DELETE" },
      ];
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
    onClickAction: function (action) {
      console.log(action);
      this.showMove = true;
    },
  },
};
</script>
<style >
</style>

