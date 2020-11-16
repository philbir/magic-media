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
      <FilterList
        :items="persons"
        title="Person"
        max-height="250"
        value-field="id"
        text-field="name"
        @change="onSelectPerson"
      ></FilterList>
    </div>
    <div v-if="activeTabId == 'geo'">
      <FilterList
        :items="countries"
        title="Countries"
        max-height="250"
        @change="onSelectCountry"
      >
        <template v-slot:default="{ item }">
          <v-list-item-title
            class="list-country"
            :style="{
              'background-image':
                'url(https://www.countryflags.io/' +
                item.value +
                '/shiny/64.png',
            }"
            >{{ item.display }}</v-list-item-title
          >
        </template>
      </FilterList>
      <FilterList
        :items="cities"
        title="Cities"
        max-height="250"
        @change="onSelectCity"
      ></FilterList>
    </div>
    <div v-if="activeTabId == 'other'">
      <FilterList
        :multiple="false"
        :items="albums"
        title="Albums"
        max-height="250"
        value-field="id"
        text-field="title"
        @change="onSelectAlbum"
      ></FilterList>
    </div>
  </div>
</template>

<script>
import FolderTree from "./FolderTree";
import FilterList from "./Common/FilterList";

export default {
  components: { FolderTree, FilterList },
  created() {
    this.$store.dispatch("media/getSearchFacets");
  },
  data() {
    return {
      selectedPersons: [],
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
    countries: function () {
      return this.$store.state.media.facets.country;
    },
    cities: function () {
      return this.$store.state.media.facets.city;
    },
    albums: function () {
      return this.$store.state.album.allAlbums;
    },
  },
  methods: {
    select: function (tab) {
      this.activeTabId = tab.id;
    },
    onSelectPerson: function (persons) {
      const selected = persons.map((x) => x.id);
      this.$store.dispatch("media/setPersonFilter", selected);
    },
    onSelectCity: function (cities) {
      const selected = cities.map((x) => x.value);
      this.$store.dispatch("media/setCityFilter", selected);
    },
    onSelectCountry: function (countries) {
      const selected = countries.map((x) => x.value);
      this.$store.dispatch("media/setCountryFilter", selected);
    },
    onSelectAlbum: function (album) {
      this.$store.dispatch("media/setAlbumFilter", album ? album.id : null);
    },
  },
};
</script>

<style scoped>
.list-country {
  background-repeat: no-repeat;
  background-size: contain;
  background-position-x: 142px;
}
</style>
