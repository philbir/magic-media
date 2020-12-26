<template>
  <div>
    <slot v-if="ready"></slot>
    <slot v-if="isRootSlot" name="error"></slot>
    <v-app v-if="me == null">
      <div style="width: 100%; height: 60vh">
        <v-container fill-height fluid>
          <v-row align="center" justify="center">
            <v-col class="d-flex justify-center">
              <div class="text-center">
                <v-progress-circular
                  :size="28"
                  color="blue"
                  indeterminate
                ></v-progress-circular>
                {{ message }}
              </div>
            </v-col>
          </v-row>
        </v-container>
      </div>
    </v-app>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  data: () => ({
    message: "Authenticating user...",
  }),
  computed: {
    ...mapState("user", ["me", "error"]),
    ready: function () {
      return this.me != null && !this.$route.meta.isRoot;
    },
    isRootSlot: function () {
      return this.error || this.$route.meta.isRoot;
    },
  },
  created() {
    this.$store.dispatch("user/getMe");
  },
  watch: {
    me: function (newValue) {
      if (newValue) {
        this.message = "Loading data...";
        this.$store.dispatch("person/getAll").then(() => {
          this.preloaded = true;
        });
        this.$store.dispatch("person/getAllGroups").then(() => {
          this.preloaded = true;
        });
        this.$store.dispatch("user/getAll").then(() => {
          this.preloaded = true;
        });
        this.$store.dispatch("album/getAll");
      }
    },
    error: function (newValue) {
      if (newValue && !this.$route.meta.isRoot) {
        this.$router.push({ name: "Error" });
      }
    },
  },
};
</script>

<style>
</style>

