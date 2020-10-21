<template>
  <v-app>
    <!--
    <v-app-bar app color="primary" style="z-index: 10" light>
      <h2 class="white--text">magic media</h2>

      <v-spacer></v-spacer>
    </v-app-bar>
    -->
    <v-main>
      <router-view></router-view>
    </v-main>
  </v-app>
</template>

<script>
export default {
  name: "App",

  components: {},

  data: () => ({
    //
  }),
};
</script>
