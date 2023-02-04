<template>

<v-text-field
      class="ml-4 mr-4"
      v-model="text"
      label="Filename, Id"
      clearable
    ></v-text-field>
  </template>
  
  <script>
  import { mapActions } from "vuex";
  
  export default {
    components: {  },
    computed: {
    text: {
      set(value) {
        if (value && value.length < 3)
            return;
        this.setFilter({
          key: "text",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.text;
      },
    },
  },
    methods: {
      ...mapActions("media", ["setFilter"]),
    },
  };
  </script>
  <style>
  </style>