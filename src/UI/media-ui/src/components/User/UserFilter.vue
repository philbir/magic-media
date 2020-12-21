<template>
  <div>
    <v-text-field
      v-model="searchText"
      label="User"
      prepend-inner-icon="mdi-magnify"
    ></v-text-field>
  </div>
</template>

<script>
import { debounce } from "lodash";
export default {
  created() {},
  data() {
    return {
      searchText: "",
    };
  },
  watch: {
    searchText: debounce(function (newValue) {
      this.$store.dispatch("user/filter", {
        searchText: newValue,
      });
    }, 500),
  },
};
</script>

<style scoped>
</style>
