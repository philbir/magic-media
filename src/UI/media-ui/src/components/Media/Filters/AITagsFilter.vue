<template>
  <FilterList
    v-if="aiTags"
    :items="aiTags"
    title="AI Tags"
    max-height="250"
    v-model="selectedTags"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedTags: {
      set(value) {
        this.setFilter({
          key: "aiTags",
          value: value
        });
      },
      get() {
        return this.$store.state.media.filter.aiTags;
      }
    },
    aiTags: function() {
      return this.$store.state.media.facets.aiTags;
    }
  },
  methods: {
    ...mapActions("media", ["setFilter"])
  }
};
</script>

<style></style>
