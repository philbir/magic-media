<template>
  <v-dialog width="500" v-model="isOpen">
    <v-card elevation="2" v-if="person">
      <v-card-title> Edit {{ album.title }}</v-card-title>
      <v-card-text>
        <v-form v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="album.title"
                  label="Title"
                  required
                ></v-text-field>
              </v-col>

              <v-col cols="12" md="6"> </v-col>

              <v-col cols="12" md="12"> </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
        <v-btn color="red darken-1" text @click="deleteAlbum"> Delete </v-btn>
        <v-btn color="primary" text @click="save"> Save </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
/* eslint-disable no-debugger */
export default {
  props: {
    show: {
      type: Boolean,
    },
    albumId: {
      type: String,
    },
  },
  data() {
    return {
      menu: false,
      album: {},
      valid: true,
    };
  },
  watch: {
    albumId: function (newValue) {
      if (newValue) {
        const album = this.$store.state.album.albums.find(
          (x) => x.id === newValue
        );
        this.album = { ...album };
      } else {
        this.album = {};
      }
    },
  },
  computed: {
    isOpen: {
      get() {
        return this.show;
      },
      set(val) {
        this.$emit("close", val);
      },
    },
  },
  methods: {
    save: function () {
      this.isOpen = false;
    },
    cancel: function () {
      this.isOpen = false;
    },
    deleteAlbum: function () {
      this.isOpen = false;
    },
  },
};
</script>

<style scoped>
</style>

