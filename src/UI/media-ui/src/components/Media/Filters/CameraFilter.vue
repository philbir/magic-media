<template>
  <FilterList
    :items="persons"
    title="Person"
    max-height="250"
    value-field="id"
    text-field="name"
    v-model="selectedPersons"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {},
  methods: {
    ...mapActions("media", ["setFilter"]),
  },
};
</script>

<style>
</style>