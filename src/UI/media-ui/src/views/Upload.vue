<template>
  <div>
    <h1>Upload</h1>

    <v-file-input
      label="Upload image"
      show-size
      prepend-icon="mdi-camera"
      @change="selectFile"
    ></v-file-input>

    <v-progress-linear
      v-show="uploading"
      indeterminate
      color="yellow"
    ></v-progress-linear>
    <v-alert v-show="uploaded" dense text type="success">
      Upload sucessfull <router-link to="/">view images</router-link>
    </v-alert>
  </div>
</template>

<script>
import { uploadFile } from "../services/uploadFileService";

export default {
  data() {
    return {
      currentFile: null,
      uploading: false,
      uploaded: false,
    };
  },
  methods: {
    selectFile(file) {
      const self = this;
      this.currentFile = file;
      self.uploading = true;

      uploadFile(this.currentFile, (e) => {
        console.log(e);
      }).then((res) => {
        self.uploading = false;
        self.uploaded = true;
        console.log(res);
      });
    },
  },
};
</script>

<style scoped>
</style>

