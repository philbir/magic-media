<template>
  <div>
    <v-text-field
      v-model="searchText"
      label="Album"
      prepend-inner-icon="mdi-magnify"
    ></v-text-field>
  </div>
</template>

<script>
export default {
  components: {},
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  watch: {
    searchText: function (newValue) {
      this.$store.dispatch("album/filter", {
        searchText: newValue,
      });
    },
  },
  computed: {},

  methods: {},
};
</script>

<style scoped>
</style>
