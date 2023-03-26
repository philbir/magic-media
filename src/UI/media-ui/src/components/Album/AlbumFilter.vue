<template>
  <div>
    <v-text-field
      v-model="searchText"
      label="Album"
      prepend-inner-icon="mdi-magnify"
      clearable
    ></v-text-field>

    <FilterList
      :items="persons"
      title="Person"
      max-height="250"
      value-field="id"
      text-field="name"
      v-model="selectedPersons"
    ></FilterList>
  </div>
</template>

<script>
import FilterList from "../Common/FilterList";
import { debounce } from "lodash";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  created() {},
  data() {
    return {
      searchText: ""
    };
  },
  watch: {
    searchText: debounce(function(newValue) {
      this.filter({
        searchText: newValue
      });
    }, 300)
  },
  computed: {
    selectedPersons: {
      set(value) {
        this.filter({ persons: value });
      },
      get() {
        return this.$store.state.album.filter.persons;
      }
    },
    persons: function() {
      return this.$store.state.person.persons.map(p => {
        return {
          name: p.name,
          id: p.id
        };
      });
    }
  },
  methods: {
    ...mapActions("album", ["filter"])
  }
};
</script>

<style scoped></style>
