<template>
  <div>
    <v-text-field
      v-model="searchText"
      label="Person"
      prepend-inner-icon="mdi-magnify"
    ></v-text-field>
    <FilterList
      :items="groups"
      title="Groups"
      max-height="250"
      value-field="id"
      text-field="name"
      v-model="selectedGroups"
    ></FilterList>
  </div>
</template>

<script>
import FilterList from "../Common/FilterList";
import { debounce } from "lodash";
export default {
  components: { FilterList },
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  watch: {
    searchText: debounce(function (newValue) {
      this.$store.dispatch("person/filter", {
        searchText: newValue,
        groups: this.selectedGroups,
      });
    }, 500),
  },
  computed: {
    selectedGroups: {
      set(value) {
        this.$store.dispatch("person/filter", {
          searchText: this.searchText,
          groups: value,
        });
      },
      get() {
        return this.$store.state.media.filter.groups;
      },
    },
    groups: function () {
      return this.$store.state.person.groups;
    },
  },
  methods: {},
};
</script>

<style scoped>
</style>
