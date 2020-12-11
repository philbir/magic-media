<template>
  <v-date-picker
    class="mt-3 ml-1"
    color="blue"
    width="248"
    v-model="selectedDate"
    :max="new Date().toISOString().substr(0, 10)"
    min="1976-01-01"
    @click:year="dateYearChanged"
    @click:month="dateMonthChanged"
  ></v-date-picker>
</template>

<script>
import { mapActions } from "vuex";

export default {
  data() {
    return {
      dates: null,
    };
  },
  computed: {
    selectedDate: {
      set(value) {
        this.setFilter({
          key: "date",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.date;
      },
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
    dateYearChanged: function (year) {
      this.setFilter({
        key: "date",
        value: year.toString(),
      });
    },
    dateMonthChanged: function (month) {
      this.setFilter({
        key: "date",
        value: month,
      });
    },
  },
};
</script>

<style>
</style>