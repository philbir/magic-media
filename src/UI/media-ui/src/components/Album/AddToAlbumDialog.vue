<template>
  <v-dialog width="500" v-model="show">
    <v-card elevation="2">
      <v-card-title> Add to album </v-card-title>
      <v-card-text>
        <v-container>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-text-field
                :disabled="selectedAlbum != undefined"
                v-model="newTitle"
                label="New Album title"
                prepend-inner-icon="mdi-image-album"
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
                label="Search in Albums"
                prepend-inner-icon="mdi-magnify"
                flat
                hide-details
                clearable
              ></v-text-field
            ></v-col>
          </v-row>
          <v-row>
            <v-col cols="12">
              <v-list
                :disabled="newTitle != undefined && newTitle.length > 0"
                flat
                dense
                max-height="200"
                height="200"
                class="overflow-y-auto"
              >
                <v-list-item-group v-model="selectedAlbum">
                  <v-list-item
                    v-for="item in filteredAlbums"
                    dense
                    :key="item.id"
                    :value="item"
                  >
                    <template v-slot:default="{ active }">
                      <v-list-item-action>
                        <v-checkbox
                          :input-value="active"
                          :true-value="item.id"
                          color="primary"
                        ></v-checkbox>
                      </v-list-item-action>
                      <v-list-item-content>
                        <slot v-bind:item="item">
                          <v-list-item-title
                            >{{ item.title }}
                          </v-list-item-title>
                        </slot>
                      </v-list-item-content>
                    </template>
                  </v-list-item>
                </v-list-item-group>
              </v-list>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="close"> Close </v-btn>
        <v-btn color="primary" text @click="save"> Add to album </v-btn>
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
      newTitle: "",
      selectedAlbum: null,
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
    filteredAlbums: function () {
      return this.$store.state.album.allAlbums.filter((x) => {
        if (!this.searchText) {
          return true;
        }
        return x.title.toLowerCase().includes(this.searchText.toLowerCase());
      });
    },
  },
  methods: {
    close: function () {
      this.isOpen = false;
    },
    save: function () {
      let input = {};
      if (this.selectedAlbum) {
        input.albumId = this.selectedAlbum.id;
      } else if (this.newTitle.length > 2) {
        input.newAlbumTitle = this.newTitle;
      }

      input.mediaIds = this.$store.getters["media/selectedMediaIds"];
      this.$store.dispatch("album/addItems", input);
      this.$store.dispatch("media/clearSelected");
      this.newAlbumTitle = null;

      this.isOpen = false;
    },
  },
};
</script>

<style scoped>
</style>

