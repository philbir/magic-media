<template>
  <div v-if="loading">
    <v-progress-linear indeterminate color="blue" top />
    <div class="ma-5">Loading devices...</div>
  </div>

  <v-list flat v-else>
    <v-list-item-group v-model="selectedItem" color="primary">
      <v-list-item v-for="(device, i) in devices" :key="i">
        <v-list-item-icon>
          <v-icon :color="device.online ? 'green' : 'red'"
            >mdi-television</v-icon
          >
        </v-list-item-icon>
        <v-list-item-content>
          <v-list-item-title v-text="device.name"></v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-list-item-group>
  </v-list>
</template>

<script>
export default {
  components: {},
  created() {},
  data() {
    return {
      selectedItem: null
    };
  },
  computed: {
    devices: function() {
      return this.$store.state.samsung.devices.list;
    },
    loading: function() {
      return this.$store.state.samsung.devices.loading;
    }
  },
  methods: {}
};
</script>

<style scoped></style>
