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
      <v-list flat dense max-height="250" width="250" class="overflow-y-auto">
        <v-list-item-group
          v-model="selectedPersons"
          multiple
          @change="onSelectPerson"
        >
          <v-list-item
            dense
            v-for="person in persons"
            :key="person.id"
            :value="person"
          >
            <template v-slot:default="{ active }">
              <v-list-item-action>
                <v-checkbox
                  :input-value="active"
                  :true-value="person.id"
                  color="primary"
                ></v-checkbox>
              </v-list-item-action>

              <v-list-item-content>
                <v-list-item-title>{{ person.name }}</v-list-item-title>
              </v-list-item-content>
            </template>
          </v-list-item>
        </v-list-item-group>
      </v-list>
    </div>
    <div v-if="activeTabId == 'geo'">
      <v-card class="mx-auto" tile>
        <v-list flat dense max-height="250" width="250" class="overflow-y-auto">
          <v-subheader>Countries</v-subheader>
          <v-list-item-group
            v-model="selectedCountries"
            @change="onSelectCountry"
            multiple
          >
            <v-list-item
              style="height: 26px"
              dense
              v-for="country in countries"
              :key="country.value"
              :value="country"
            >
              <template v-slot:default="{ active }">
                <v-list-item-action>
                  <v-checkbox
                    :input-value="active"
                    :true-value="country.value"
                    color="primary"
                  ></v-checkbox>
                </v-list-item-action>

                <v-list-item-content>
                  <v-list-item-title>{{ country.text }}</v-list-item-title>
                </v-list-item-content>
              </template>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-card>
      <v-card class="mx-auto" tile width="250">
        <v-list flat dense max-height="250" class="overflow-y-auto">
          <v-subheader>Cities</v-subheader>
          <v-list-item-group
            v-model="selectedCities"
            @change="onSelectCity"
            multiple
          >
            <v-list-item
              style="height: 26px"
              dense
              v-for="city in cities"
              :key="city.value"
              :value="city"
            >
              <template v-slot:default="{ active }">
                <v-list-item-action>
                  <v-checkbox
                    :input-value="active"
                    :true-value="city.value"
                    color="primary"
                  ></v-checkbox>
                </v-list-item-action>

                <v-list-item-content>
                  <v-list-item-title>{{ city.text }}</v-list-item-title>
                </v-list-item-content>
              </template>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-card>
    </div>
  </div>
</template>

<script>
import FolderTree from "./FolderTree";

export default {
  components: { FolderTree },
  created() {
    this.$store.dispatch("media/getSearchFacets");
  },
  data() {
    return {
      selectedPersons: [],
      selectedCountries: [],
      selectedCities: [],
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
  },
  methods: {
    select: function (tab) {
      this.activeTabId = tab.id;
    },
    onSelectPerson: function () {
      var selected = this.selectedPersons.map((x) => x.id);
      this.$store.dispatch("media/setPersonFilter", selected);
    },
    onSelectCity: function () {
      var selected = this.selectedCities.map((x) => x.value);
      console.log(selected);
      this.$store.dispatch("media/setCityFilter", selected);
    },
    onSelectCountry: function () {
      var selected = this.selectedCountries.map((x) => x.value);
      console.log(selected);
      this.$store.dispatch("media/setCountryFilter", selected);
    },
  },
};
</script>

<style scoped>
</style>
