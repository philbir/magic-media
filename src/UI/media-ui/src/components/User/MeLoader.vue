<template>
  <div>
    <slot v-if="ready"></slot>
    <slot v-if="error" name="error"></slot>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  data: () => ({}),
  computed: {
    ...mapState("user", ["me", "error"]),
    ready: function () {
      return this.me != null;
    },
  },
  created() {
    this.$store.dispatch("user/getMe");
  },
  watch: {
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

