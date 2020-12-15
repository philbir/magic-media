<template>
  <v-dialog width="500" v-model="isOpen">
    <v-card elevation="2" v-if="isOpen">
      <v-card-title> Create user from {{ person.name }}</v-card-title>
      <v-card-text>
        <v-form v-model="valid">
          <v-container>
            <v-row>
              <v-col md="12">
                <v-text-field
                  v-model="email"
                  label="Email"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
        <v-btn color="primary" text @click="save"> Create </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import { mapActions } from "vuex";
export default {
  props: {
    show: {
      type: Boolean,
    },
    person: {
      type: Object,
    },
  },
  data() {
    return {
      email: null,
      valid: true,
    };
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
    ...mapActions("user", ["createFromPerson"]),
    save: function () {
      this.createFromPerson({
        personId: this.person.id,
        email: this.email,
      });
      this.isOpen = false;
    },
    cancel: function () {
      this.isOpen = false;
    },
  },
};
</script>

<style scoped>
</style>

