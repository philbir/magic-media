<template>
  <FilterList
    :items="countries"
    title="Countries"
    max-height="250"
    v-model="selectedCountries"
  >
    <template v-slot:default="{ item }">
      <v-list-item-title
        class="list-country"
        :style="{
          'background-image':
            'url(https://www.countryflags.io/' + item.value + '/shiny/64.png',
        }"
        >{{ item.display }}</v-list-item-title
      >
    </template>
  </FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedCountries: {
      set(value) {
        this.setFilter({
          key: "countries",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.cities;
      },
    },
    countries: function () {
      return this.$store.state.media.facets.country;
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
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
