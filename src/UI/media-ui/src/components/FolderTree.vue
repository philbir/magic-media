<template>
  <v-treeview
    activatable
    selection-type="independent"
    :multiple-active="false"
    :open="initiallyOpen"
    :items="folderTree"
    dense
    item-key="name"
    :open-on-click="false"
  >
    <template v-slot:prepend="{ open }">
      <v-icon color="blue">
        {{ open ? "mdi-folder-open" : "mdi-folder" }}
      </v-icon>
    </template>
  </v-treeview>
</template>

<script>
export default {
  created() {
    this.$store.dispatch("media/getFolderTree");
  },
  data: () => ({
    initiallyOpen: ["Home"],
    files: {},
    tree: [],
  }),
  computed: {
    folderTree: function () {
      return [this.$store.state.media.folderTree];
    },
  },
};
</script>

<style>
</style>
