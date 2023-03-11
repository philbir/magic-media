<template>
  <v-dialog width="500" v-model="show">
    <v-card elevation="2">
      <v-card-title> Export </v-card-title>
      <v-card-text>
        <v-container>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-text-field
                v-model="path"
                label="Path"
                prepend-inner-icon="mdi-folder"
                flat
                hide-details
                clearable
              ></v-text-field
            ></v-col>
          </v-row>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-list flat two-line>
                <v-subheader>Choose profile to export </v-subheader>
                <v-list-item-group>
                  <v-list-item
                    v-for="profile in profiles"
                    :key="profile.id"
                    @click="() => handleProfileClick(profile)"
                  >
                    <template v-slot:default>
                      <v-list-item-content>
                        <v-list-item-title>{{
                          profile.name
                        }}</v-list-item-title>
                        <v-list-item-subtitle
                          >{{ profile.location.type }} ->
                          {{ profile.location.path }}</v-list-item-subtitle
                        >
                      </v-list-item-content>
                    </template>
                  </v-list-item>
                </v-list-item-group>
              </v-list></v-col
            >
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="close"> Close </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
export default {
  props: {
    show: {
      type: Boolean
    }
  },
  data() {
    return {
      path: ""
    };
  },
  created() {
    this.$store.dispatch("user/getExportProfiles");
  },
  computed: {
    isOpen: {
      get() {
        return this.show;
      },
      set(val) {
        this.$emit("close", val);
      }
    },
    profiles: function() {
      return this.$store.state.user.exportProfiles;
    }
  },
  methods: {
    close: function() {
      this.isOpen = false;
    },
    handleProfileClick: function(profile) {
      this.$store.dispatch("media/exportSelected", {
        profileId: profile.id,
        path: this.path
      });
      this.close();
    }
  }
};
</script>

<style scoped></style>
