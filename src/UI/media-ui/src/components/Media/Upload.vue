<template>
  <v-dialog width="400" v-model="open">
    <v-card elevation="2" outlined>
      <v-card-title> Upload </v-card-title>
      <v-card-text>
        <v-container>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-file-input
                ref="upload"
                label="Upload image"
                show-size
                prepend-icon="mdi-camera"
                @change="selectFile"
              ></v-file-input>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="close"> Close </v-btn>
      </v-card-actions>
      <v-progress-linear
        v-if="uploading"
        indeterminate
        color="blue"
        top
      ></v-progress-linear>
    </v-card>
  </v-dialog>
</template>

<script>
import { uploadFile } from "../../services/uploadFileService";

export default {
  computed: {
    open: function () {
      return this.$store.state.media.uploadDialog.open;
    },
  },
  data() {
    return {
      currentFile: null,
      uploading: false,
      uploaded: false,
    };
  },
  methods: {
    close: function () {
      this.$store.dispatch("media/toggleUploadDialog", false);
    },
    selectFile(file) {
      const self = this;
      this.currentFile = file;

      if (!file) return;

      self.uploading = true;

      uploadFile(this.currentFile, () => {})
        .then(() => {
          self.uploading = false;
          self.uploaded = true;
          this.$magic.snack("Uploaded: " + this.currentFile.name, "SUCCESS");
          this.$refs.upload.reset();
        })
        .catch(() => {
          this.$magic.snack(
            "Upload failed for: " + this.currentFile.name,
            "ERROR"
          );
          self.uploading = false;
          this.$refs.upload.reset();
        });
    },
  },
};
</script>

<style scoped>
</style>

