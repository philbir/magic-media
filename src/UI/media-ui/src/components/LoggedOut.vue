<template>
  <div style="width: 100%; height: 60vh">
    <v-container fill-height fluid>
      <v-row align="center" justify="center">
        <v-col class="d-flex justify-center">
          <v-alert
            class="text-center"
            border="top"
            colored-border
            type="success"
            elevation="2"
            width="400"
          >
            You are now logged out.<br />
            <v-btn class="mt-8" @click="login">Login </v-btn>
          </v-alert></v-col
        >
      </v-row>
    </v-container>
  </div>
</template>

<script>
export default {
  created() {
  },
  methods: {
    login: function () {
      window.location.href = "/api/session/auth";
    },
  },
};
</script>

<style>
</style>