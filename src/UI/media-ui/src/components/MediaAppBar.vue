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
    <h4 class="white--text mr-4">{{ mediaCount }}</h4>

    <v-icon color="white" class="mr-2" @click="openUpload">
      mdi-cloud-upload-outline
    </v-icon>
    <Upload :show="showUpload"></Upload>
  </v-app-bar>
</template>

<script>
import Upload from "./Upload";
import AppBarNavMenu from "./AppBarNavMenu";

export default {
  name: "App",
  components: { Upload, AppBarNavMenu },

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
    mediaCount: function () {
      return this.$store.state.media.list.length;
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
  },
};
</script>
<style >
</style>

