<template>
  <div>
    <v-dialog width="500" v-model="isOpen">
      <v-card elevation="2" v-if="user">
        <v-card-title> Edit {{ user.name }}</v-card-title>
        <v-card-text>
          <v-form v-model="valid">
            <v-container>
              <v-row>
                <v-col md="6">
                  <v-text-field
                    v-model="user.name"
                    label="Name"
                    required
                  ></v-text-field>
                </v-col>

                <v-col md="6">
                  <v-text-field
                    v-model="user.email"
                    label="Email"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12" md="12">
                  <v-autocomplete
                    v-model="user.roles"
                    :items="roles"
                    text-color="white"
                    label="Roles"
                    chips
                    multiple
                    deletable-chips
                  >
                  </v-autocomplete>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions class="pa-1">
          <v-btn color="blue darken-1" text @click="onClickCreateInvite">
            Create Invite
          </v-btn>

          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
          <v-btn color="primary" text @click="save"> Save </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>
<script>
export default {
  components: {},
  props: {
    show: {
      type: Boolean,
    },
    userId: {
      type: String,
    },
  },
  data() {
    return {
      date: null,
      user: {},
      valid: true,
    };
  },
  watch: {
    userId: function (newValue) {
      console.log(newValue);
      if (newValue) {
        this.user = this.$store.state.user.list.find((x) => x.id === newValue);
      } else {
        this.user = {};
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
    roles: function () {
      return ["Admin"];
    },
  },
  methods: {
    save: function () {
      this.$store.dispatch("user/update", this.user);
      this.isOpen = false;
    },
    cancel: function () {
      this.isOpen = false;
    },
    removeRole: function (role) {
      console.log(role);
    },
    onClickCreateInvite: function () {
      this.$store.dispatch("user/createInvite", this.user.id);
    },
  },
};
</script>

<style scoped>
</style>

