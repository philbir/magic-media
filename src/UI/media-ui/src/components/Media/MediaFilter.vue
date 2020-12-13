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

    <date-filter v-if="activeTabId == 'date'"></date-filter>

    <div v-if="activeTabId == 'person'">
      <person-filter></person-filter>
    </div>

    <div v-if="activeTabId == 'geo'">
      <map-filter></map-filter>

      <country-filter></country-filter>
      <city-filter></city-filter>
    </div>
    <div v-if="activeTabId == 'other'">
      <AITagsFilter></AITagsFilter>

      <AIObjectsFilter></AIObjectsFilter>
      <album-filter></album-filter>

      <media-type-filter></media-type-filter>
    </div>
  </div>
</template>

<script>
import FolderTree from "../FolderTree";
import { mapActions } from "vuex";
import PersonFilter from "./Filters/PersonFilter.vue";
import CityFilter from "./Filters/CityFilter.vue";
import CountryFilter from "./Filters/CountryFilter.vue";
import AlbumFilter from "./Filters/AlbumFilter.vue";
import DateFilter from "./Filters/DateFilter.vue";
import MapFilter from "./Filters/MapFilter.vue";
import MediaTypeFilter from "./Filters/MediaTypeFilter.vue";
import AITagsFilter from "./Filters/AITagsFilter.vue";
import AIObjectsFilter from "./Filters/AIObjectsFilter.vue";

export default {
  components: {
    FolderTree,
    PersonFilter,
    CityFilter,
    CountryFilter,
    AlbumFilter,
    DateFilter,
    MapFilter,
    MediaTypeFilter,
    AITagsFilter,
    AIObjectsFilter,
  },
  created() {
    this.$store.dispatch("media/getSearchFacets");
  },
  data() {
    return {
      selectedMediaTypes: ["IMAGE", "VIDEO"],
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
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    select: function (tab) {
      this.activeTabId = tab.id;
    },
  },
};
</script>