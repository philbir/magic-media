<template>
  <v-dialog width="500" v-model="isOpen">
    <v-card elevation="2">
      <v-card-title> {{ title }}</v-card-title>
      <v-card-text>
        <v-container>
          <v-row v-if="context === 'QUERY'">
            <v-chip
              v-for="desc in filterDescriptions"
              :key="desc.key"
              class="ma-2"
              small
              text-color="white"
              color="blue darken-4"
            >
              {{ desc.name }} : {{ desc.description }}
            </v-chip>
          </v-row>
          <v-row v-if="context === 'FOLDER'">
            {{ folder }}
          </v-row>
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
          <div v-if="context !== 'QUERY'">
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
          </div>
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
import { mapActions, mapGetters } from "vuex";
export default {
  props: {
    show: {
      type: Boolean,
    },
    context: String,
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
    ...mapGetters("media", ["selectedMediaIds", "filterDescriptions"]),
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
    folder: function () {
      return this.$store.state.media.filter.folder;
    },
    title: function () {
      switch (this.context) {
        case "IDS":
          return `Add ${this.$store.state.media.selectedIndexes.length} items to album`;
        case "FOLDER":
          return "Add folder to album";
        case "QUERY":
          return "Create album from filters";
        default:
          return "";
      }
    },
  },
  methods: {
    ...mapActions("album", { addItemsToAlbum: "addItems" }),

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

      switch (this.context) {
        case "IDS":
          input.mediaIds = this.selectedMediaIds;
          break;
        case "FOLDER":
          input.folders = [this.$store.state.media.filter.folder];
          break;
        case "QUERY":
          input.filters = this.filterDescriptions.map((x) => {
            return {
              name: x.name,
              key: x.key,
              value: x.stringValue,
              description: x.description,
            };
          });
          break;
      }
      this.addItemsToAlbum(input);
      this.newAlbumTitle = null;

      if (this.context === "IDS") {
        this.$store.dispatch("media/clearSelected");
      }

      this.isOpen = false;
    },
  },
};
</script>

<style scoped>
</style>

