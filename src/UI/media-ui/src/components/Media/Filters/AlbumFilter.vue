<template>
  <FilterList
    v-if="albums.length > 0"
    :multiple="false"
    :items="albums"
    title="Albums"
    max-height="250"
    value-field="id"
    text-field="title"
    v-model="selectedAlbum"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedAlbum: {
      set(value) {
        this.setFilter({
          key: "albumId",
          value: value !== undefined ? value : null,
        });
      },
      get() {
        return this.$store.state.media.filter.albumId;
      },
    },
    albums: function () {
      return this.$store.state.album.allAlbums;
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
  },
};
</script>

<style>
</style>