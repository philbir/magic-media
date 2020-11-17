<template>
  <div class="ma-2">
    <h5>Map Search</h5>
    <div class="ml-2 mt-6">
      <gmap-autocomplete
        placeholder="Search"
        @place_changed="setPlace"
        :select-first-on-enter="true"
      >
      </gmap-autocomplete>
    </div>
  </div>
</template>

<script>
export default {
  methods: {
    setPlace: function (place) {
      this.$store.dispatch("map/setPlace", place);
    },
  },
};
</script>

<style>
</style>