<template>
  <div>
    <user-filter></user-filter>

    <div class="ml-6">
      <v-switch
        v-model="failedOnly"
        color="primary"
        label="Failed only"
        @change="failedOnlyChanged"
      ></v-switch>
    </div>
  </div>
</template>

<script>
import { mapActions } from "vuex";
import UserFilter from "./Filters/UserFilter";
export default {
  components: { UserFilter },
  data() {
    return {
      failedOnly: false,
    };
  },
  methods: {
    ...mapActions("audit", ["filter"]),
    failedOnlyChanged: function (value) {
      if (value) {
        this.filter({
          success: false,
        });
      } else {
        this.filter({
          success: null,
        });
      }
    },
  },
};
</script>

<style>
</style>