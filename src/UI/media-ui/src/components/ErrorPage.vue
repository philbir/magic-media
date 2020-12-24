<template>
  <div style="width: 100%; height: 60vh">
    <v-container fill-height fluid>
      <v-row align="center" justify="center">
        <v-col class="d-flex justify-center">
          <v-alert
            class="text-center"
            border="top"
            colored-border
            type="error"
            elevation="2"
            width="500"
          >
            Sorry that this happened! 😢 But an error prevents the app from
            loading. <br />
            <v-btn class="mt-8" @click="retry">Try again </v-btn>
          </v-alert></v-col
        >
      </v-row>
    </v-container>
  </div>
</template>

<script>
export default {
  methods: {
    retry: function () {
      this.$store.dispatch("user/resetError");
      window.location.href = "/";
    },
  },
};
</script>

<style>
</style>