<template>
  <v-dialog width="600" v-model="show">
    <v-card elevation="2">
      <v-card-title> Move </v-card-title>
      <v-card-text>
        <v-container>
          <v-row dense>
            <v-col sm="8">
              <v-text-field
                v-model="newLocation"
                label="New location"
                prepend-inner-icon="mdi-folder"
                flat
                hide-details
                clearable
              ></v-text-field
            ></v-col>
            <v-col sm="4">
              <v-select
                :items="rules"
                v-model="rule"
                clearable
                label="Rule"
              ></v-select>
            </v-col>
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
              <div style="height: 200px; overflow-y: auto">
                <v-treeview
                  activatable
                  selection-type="independent"
                  :multiple-active="false"
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
            </v-col>
          </v-row>
          <v-row>
            <v-col sm="12"
              ><v-chip
                v-for="path in recentMoves"
                :key="path"
                class="mr-2"
                color="primary"
                label
                small
                @click="moveRecent(path)"
                >{{ path }}</v-chip
              ></v-col
            >
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="close"> Close </v-btn>
        <v-btn color="primary" text @click="save"> Move </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
export default {
  props: {
    show: {
      type: Boolean
    }
  },
  data() {
    return {
      searchText: "",
      newLocation: null,
      rules: ["YearAndMonth"],
      rule: null
    };
  },
  computed: {
    isOpen: {
      get() {
        return this.show;
      },
      set(val) {
        this.$emit("close", val);
      }
    },
    folderTree: function() {
      return this.$store.state.media.folderTree.children;
    },
    recentMoves: function() {
      return this.$store.state.media.recentMoves;
    }
  },
  methods: {
    close: function() {
      this.isOpen = false;
    },
    moveRecent: function(path) {
      this.newLocation = path;
      this.save();
    },
    save: function() {
      this.$store.dispatch("media/moveSelected", {
        newLocation: this.newLocation,
        rule: this.rule
      });
      this.searchText = "";
      this.newLocation = null;
      this;
      this.close();
    },
    onSelect(e) {
      if (e.length > 0) {
        this.newLocation = e[0].path;
      }
    }
  }
};
</script>

<style scoped></style>
