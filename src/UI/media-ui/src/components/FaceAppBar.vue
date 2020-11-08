<template>
  <v-app-bar app dense clipped-left color="indigo darken-4">
    <AppBarNavMenu></AppBarNavMenu>

    <v-switch
      dense
      @change="toggleEditMode"
      color="info"
      value="edit"
      class="mt-4 ml-2 d-none d-md-block"
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
        <v-list-item v-for="action in faceActions" :key="action.text">
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
  </v-app-bar>
</template>

<script>
import AppBarNavMenu from "./AppBarNavMenu";

export default {
  name: "App",
  components: { AppBarNavMenu },

  data: () => ({}),
  computed: {
    faceActions: function () {
      return [{ text: "Assign" }, { text: "Approve" }, { text: "Remove" }];
    },
    editModeText: function () {
      return this.$store.state.face.isEditMode ? "Edit" : "View";
    },
    totalLoaded: function () {
      return this.$store.state.face.totalLoaded;
    },
    totalCount: function () {
      return this.$store.state.face.totalCount;
    },
    selectedCount: function () {
      return this.$store.state.face.selectedIndexes.length;
    },
    loading: function () {
      return this.$store.state.face.listLoading;
    },
  },
  methods: {
    toggleEditMode: function (value) {
      this.$store.dispatch("face/toggleEditMode", value === "edit");
    },
    selectAll: function () {
      this.$store.dispatch("face/selectAll");
    },
  },
};
</script>
<style >
</style>

