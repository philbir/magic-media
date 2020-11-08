vvv<template>
  <v-treeview
    activatable
    selection-type="independent"
    :multiple-active="false"
    :open="initiallyOpen"
    :items="folderTree"
    dense
    item-key="name"
    :return-object="true"
    :open-on-click="false"
    @update:active="onSelect"
  >
    <template v-slot:prepend="{ open, item }">
      <v-icon v-if="item.name === 'Home'"> mdi-home </v-icon>
      <v-icon v-else small color="blue">
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
      return this.$store.state.media.folderTree.children;
    },
  },
  methods: {
    onSelect(e) {
      console.log(e);
      if (e.length > 0) {
        this.$store.dispatch("media/setFolderFilter", e[0].path);
      }
    },
  },
};
</script>

<style>
</style>
