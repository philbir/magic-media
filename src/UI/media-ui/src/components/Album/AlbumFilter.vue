<template>
  <div>
    <v-text-field
      v-model="searchText"
      label="Album"
      prepend-inner-icon="mdi-magnify"
    ></v-text-field>

    <FilterList
      :items="persons"
      title="Person"
      max-height="250"
      value-field="id"
      text-field="name"
      @change="onSelectPerson"
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
      searchText: "",
      selectedPersons: [],
    };
  },
  watch: {
    searchText: debounce(function (newValue) {
      this.filter({
        searchText: newValue,
      });
    }, 300),
  },
  computed: {
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
    ...mapActions("album", ["filter"]),

    onSelectPerson: function (persons) {
      const selected = persons.map((x) => x.id);
      this.filter({ persons: selected });
    },
  },
};
</script>

<style scoped>
</style>
