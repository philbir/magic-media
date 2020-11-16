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
    searchText: function (newValue) {
      this.$store.dispatch("album/filter", {
        searchText: newValue,
      });
    },
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
    onSelectPerson: function (persons) {
      const selected = persons.map((x) => x.id);
      this.$store.dispatch("album/filter", { persons: selected });
    },
  },
};
</script>

<style scoped>
</style>
