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
      @change="onSelectGroup"
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
      selectedGroups: [],
      searchText: "",
    };
  },
  watch: {
    searchText: function (newValue) {
      this.$store.dispatch("person/filter", {
        searchText: newValue,
        groups: this.selectedGroups,
      });
    },
  },
  computed: {
    groups: function () {
      return this.$store.state.person.groups;
    },
  },

  methods: {
    onSelectGroup: function (groups) {
      const selected = groups.map((x) => x.id);
      this.$store.dispatch("person/filter", selected);
    },
  },
};
</script>

<style scoped>
</style>
