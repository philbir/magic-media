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
      @change="dateChanged"
      @click:year="dateYearChanged"
      @click:month="dateMonthChanged"
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
      <GmapMap
        :center="mapCenter"
        :zoom="7"
        :options="mapOptions"
        ref="mapRef"
        map-type-id="roadmap"
        style="width: 100%; height: 200px"
        @click="clickMap"
      >
        <GmapMarker v-if="mapMarker" :position="mapMarker"
      /></GmapMap>

      <v-text-field
        class="ml-4 mr-4"
        v-model="mapRadius"
        label="Radius"
        suffix="km"
        @change="onRadiusChange"
        prepend-inner-icon="mdi-map-marker-radius-outline"
      ></v-text-field>
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

      <v-card flat>
        <v-card-text>
          <v-row align="center">
            <v-subheader>Media Type</v-subheader>
          </v-row>
          <v-row>
            <v-btn-toggle
              v-model="selectedMediaTypes"
              multiple
              @change="onSelectMediaType"
            >
              <v-btn small value="IMAGE">
                <v-icon>mdi-image</v-icon>
              </v-btn>
              <v-btn small value="VIDEO">
                <v-icon>mdi-video</v-icon>
              </v-btn>
            </v-btn-toggle>
          </v-row>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<script>
import FolderTree from "../FolderTree";
import FilterList from "../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FolderTree, FilterList },
  created() {
    this.$store.dispatch("media/getSearchFacets");
  },
  data() {
    return {
      selectedPersons: [],
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
      mapOptions: {
        zoomControl: false,
        mapTypeControl: false,
        scaleControl: false,
        streetViewControl: false,
        rotateControl: false,
        fullscreenControl: false,
        disableDefaultUi: false,
      },
      mapMarker: null,
      mapRadius: 10,
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
    mapCenter: function () {
      return { lat: 47.28752423541094, lng: 8.533110649108862 };
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
    filterDesc: function () {
      return this.$store.getters["media/filterDescriptions"];
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    select: function (tab) {
      this.activeTabId = tab.id;
    },
    onSelectPerson: function (persons) {
      const selected = persons.map((x) => x.id);
      this.setFilter({
        key: "persons",
        value: selected,
      });
    },
    onSelectCity: function (cities) {
      const selected = cities.map((x) => x.value);
      this.setFilter({
        key: "cities",
        value: selected,
      });
    },
    onSelectCountry: function (countries) {
      const selected = countries.map((x) => x.value);
      this.setFilter({
        key: "countries",
        value: selected,
      });
    },
    onSelectAlbum: function (album) {
      this.setFilter({
        key: "albumId",
        value: album ? album.id : null,
      });
    },
    onSelectMediaType: function (types) {
      this.setFilter({
        key: "mediaTypes",
        value: types,
      });
    },
    clickMap: function (loc) {
      this.mapMarker = {
        lat: loc.latLng.lat(),
        lng: loc.latLng.lng(),
      };

      this.setSearchFilter();
    },
    onRadiusChange: function () {
      this.setSearchFilter();
    },
    setSearchFilter: function () {
      if (this.mapMarker) {
        const geoFilter = {
          latitude: this.mapMarker.lat,
          longitude: this.mapMarker.lng,
          distance: +this.mapRadius,
        };
        this.setFilter({
          key: "geoRadius",
          value: geoFilter,
        });

        //this.$store.dispatch("media/setGeoFilter", geoFilter);
      }
    },
    dateChanged: function (value) {
      this.setFilter({
        key: "date",
        value: value,
      });
    },
    dateYearChanged: function (year) {
      this.setFilter({
        key: "date",
        value: year.toString(),
      });
    },
    dateMonthChanged: function (month) {
      this.setFilter({
        key: "date",
        value: month,
      });
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
