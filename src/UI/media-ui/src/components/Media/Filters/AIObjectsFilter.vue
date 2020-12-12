<template>
  <FilterList
    :items="objects"
    title="Objects"
    max-height="250"
    v-model="selectedObjects"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedObjects: {
      set(value) {
        this.setFilter({
          key: "objects",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.objects;
      },
    },
    objects: function () {
      return this.$store.state.media.facets.aiObjects;
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
  },
};
</script>

<style>
</style>