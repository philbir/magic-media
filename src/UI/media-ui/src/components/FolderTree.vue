<template>
  <div>
    <v-text-field
      v-model="searchText"
      label="Search in folders"
      prepend-inner-icon="mdi-magnify"
      flat
      hide-details
      clearable
    ></v-text-field>
    <v-treeview
      activatable
      selection-type="independent"
      :multiple-active="false"
      :open="initiallyOpen"
      :items="folderTree"
      :search="searchText"
      dense
      item-key="name"
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
export default {
  created() {
    this.$store.dispatch("media/getFolderTree");
  },
  data: () => ({
    initiallyOpen: ["Home"],
    files: {},
    specialFolders: [
      {
        name: "Favorites",
        path: "SPECIAL:Favorites",
        icon: "mdi-heart",
        color: "red",
      },
      {
        name: "Deleted",
        path: "SPECIAL:Deleted",
        icon: "mdi-trash-can-outline",
        color: "black",
      },
      {
        name: "Recently added",
        path: "SPECIAL:RecentAdded",
        icon: "mdi-history",
        color: "black",
      },
    ],
    searchText: "",
  }),
  computed: {
    folderTree: function () {
      return [
        ...this.$store.state.media.folderTree.children,
        ...this.specialFolders,
      ];
    },
  },
  methods: {
    onSelect(e) {
      if (e.length > 0) {
        this.$store.dispatch("media/setFolderFilter", e[0].path);
      }
    },
  },
};
</script>

<style>
</style>
