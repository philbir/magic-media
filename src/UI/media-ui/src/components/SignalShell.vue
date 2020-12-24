<template>
  <div v-if="false"></div>
</template>

<script>
import { mediaOperationTypeMap } from "../services/mediaOperationService";

export default {
  created() {
    const self = this;

    this.$socket.start();

    this.$socket.on("mediaOperationCompleted", (data) => {
      self.$store.dispatch("snackbar/mediaOperationCompleted", data);
    });
    this.$socket.on("mediaOperationRequestCompleted", (data) => {
      self.$store.dispatch("snackbar/mediaOperationRequestCompleted", data);

      this.$store.dispatch("media/getFolderTree");

      var opType = mediaOperationTypeMap[data.type];

      this.$magic.snack(
        `${opType.completedText} (${data.successCount} items)`,
        "SUCCESS"
      );
    });
  },
};
</script>

<style>
</style>