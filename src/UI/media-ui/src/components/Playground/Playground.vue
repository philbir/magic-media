<template>
  <v-card class="mx-auto" max-width="300">
    <span v-touch:tap="touchHandler">Tap Me</span>
  </v-card>
</template>

<script>
export default {
  data() {
    return {
      foo: "bar",
    };
  },
  methods: {
    touchHandler: function () {},
  },
};
</script>

<style>
.player-container {
  height: 80vh;
  background-color: #000;
  font-weight: normal;
}
</style>