<template>
  <div>
    <v-row>
      <v-btn
        v-for="tab in tabs"
        :key="tab.id"
        class="ml-1"
        :color="tab.color"
        small
        @click="select(tab)"
      >
        <v-icon> {{ tab.icon }} </v-icon>
      </v-btn>
    </v-row>

    <FolderTree v-if="activeTabId == 'folder'"></FolderTree>

    <v-date-picker
      class="mt-3 ml-1"
      color="blue"
      width="248"
      v-if="activeTabId == 'date'"
      v-model="dates"
      range
    ></v-date-picker>

    <div v-if="activeTabId == 'person'">
      <v-list flat dense>
        <v-list-item-group v-model="settings" multiple>
          <v-list-item dense v-for="person in persons" :key="person.id">
            <template v-slot:default="{ active }">
              <v-list-item-action>
                <v-checkbox :input-value="active" color="primary"></v-checkbox>
              </v-list-item-action>

              <v-list-item-content>
                <v-list-item-title>{{ person.name }}</v-list-item-title>
              </v-list-item-content>
            </template>
          </v-list-item>
        </v-list-item-group>
      </v-list>
    </div>
  </div>
</template>

<script>
import FolderTree from "./FolderTree";

export default {
  components: { FolderTree },
  data() {
    return {
      settings: [],
      activeTabId: "folder",
      dates: null,
      tabDef: [
        {
          id: "folder",
          icon: "mdi-folder-outline",
        },
        {
          id: "geo",
          icon: " mdi-earth",
        },
        {
          id: "person",
          icon: " mdi-account-outline",
        },
        {
          id: "date",
          icon: "mdi-calendar-range-outline",
        },
        {
          id: "other",
          icon: " mdi-dots-horizontal",
        },
      ],
    };
  },
  computed: {
    tabs: function () {
      return this.tabDef.map((tab) => {
        if (tab.id === this.activeTabId) {
          tab.color = "blue lighten-4";
        } else {
          tab.color = "white";
        }
        return tab;
      });
    },
    persons: function () {
      return this.$store.state.person.persons.map((p) => {
        return {
          name: p.name,
          id: p.id,
        };
      });
    },
  },
  methods: {
    select: function (tab) {
      this.activeTabId = tab.id;
    },
  },
};
</script>
