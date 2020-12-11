<template>
  <FilterList
    :items="cities"
    title="Cities"
    max-height="250"
    v-model="selectedCities"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedCities: {
      set(value) {
        console.log(value);
        this.setFilter({
          key: "cities",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.cities;
      },
    },
    cities: function () {
      return this.$store.state.media.facets.city;
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
  },
};
</script>

<style>
</style>