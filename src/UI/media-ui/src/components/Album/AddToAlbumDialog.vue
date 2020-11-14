<template>
  <v-dialog width="500" v-model="show">
    <v-card elevation="2">
      <v-card-title> Move </v-card-title>
      <v-card-text>
        <v-container>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-text-field
                v-model="newTitle"
                label="New Album title"
                prepend-inner-icon="mdi-folder"
                flat
                hide-details
                clearable
              ></v-text-field
            ></v-col>
          </v-row>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-text-field
                v-model="searchText"
                label="Search in folders"
                prepend-inner-icon="mdi-magnify"
                flat
                hide-details
                clearable
              ></v-text-field
            ></v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <div style="height: 200px; overflow-y: auto"></div>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="close"> Close </v-btn>
        <v-btn color="primary" text @click="add"> Move </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
export default {
  props: {
    show: {
      type: Boolean,
    },
  },
  created() {
    this.$store.dispatch("album/getAll");
  },
  data() {
    return {
      searchText: "",
      newTitle: null,
    };
  },
  computed: {
    isOpen: {
      get() {
        return this.show;
      },
      set(val) {
        this.$emit("close", val);
      },
    },
    folderTree: function () {
      return this.$store.state.media.folderTree.children;
    },
  },
  methods: {
    close: function () {
      this.isOpen = false;
    },
    add: function () {
      this.$store.dispatch("album/addItems", this.newLocation);
      this.isOpen = false;
    },
  },
};
</script>

<style scoped>
</style>

