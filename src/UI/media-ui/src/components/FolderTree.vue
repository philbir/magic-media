<template>
  <div>
    <v-text-field
      v-model="searchText"
      @input="handleSearch"
      label="Search in folders"
      prepend-inner-icon="mdi-magnify"
      flat
      hide-details
      clearable
    ></v-text-field>
    <v-treeview
      activatable
      ref="tree"
      :multiple-active="false"
      :open="initiallyOpen"
      :items="folderTree"
      :search="searchText"
      dense
      item-key="path"
      :return-object="true"
      :open-on-click="false"
      @update:active="onSelect"
    >
      <template v-slot:prepend="{ open, item }">
        <v-icon small :color="item.color" v-if="item.icon">
          {{ item.icon }}
        </v-icon>
        <v-icon v-else small :color="open ? '#FBC02D' : '#FDD835'">
          {{ open ? "mdi-folder-open" : "mdi-folder" }}
        </v-icon>
      </template>
    </v-treeview>
  </div>
</template>

<script>
import { mapActions } from "vuex";
import { debounce } from "lodash";

export default {
  created() {
    this.$store.dispatch("media/getFolderTree");
    this.handleSearch = debounce(this.handleSearch, 500);
  },
  data: () => ({
    initiallyOpen: ["Home"],
    files: {},
    active: [],
    specialFolders: [
    {
        name: "Inbox",
        path: "New",
        icon: "mdi-mailbox-up",
        color: "blue"
      },
      {
        name: "Favorites",
        path: "SPECIAL:FAVORITES",
        icon: "mdi-heart",
        color: "red",
      },
      {
        name: "Recently added",
        path: "SPECIAL:RECENTLY_ADDED",
        icon: "mdi-history",
        color: "black",
      },
      {
        name: "Recycle Bin",
        path: "SPECIAL:RECYCLED",
        icon: "mdi-trash-can-outline",
        color: "black",
      },
    ],
    searchText: "",
  }),
  computed: {
    folderTree: function () {
      return [
        ...this.$store.state.media.folderTree.children.filter(x => x.name !== "New"),
        ...this.specialFolders,
      ];
    },
    activePath: function () {
      return this.$store.state.media.filter.folder;
    },
  },
  watch: {
    activePath: function (newValue) {
      this.active = [newValue]; //Can not sync selected node from $store 😢
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    handleSearch(input) {
      if (input) {
        if (input.length > 2) this.$refs.tree.updateAll(true);
      } else {
        this.$refs.tree.updateAll(false);
      }
    },
    onSelect(e) {
      if (e.length > 0) {
        this.selected = [e[0].path];
        this.setFilter({
          key: "folder",
          value: e[0].path,
        });
      } else {
        this.setFilter({
          key: "folder",
          value: null,
        });
      }
    },
  },
};
</script>

<style>
</style>
