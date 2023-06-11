<template>
  <div class="ma-8">
    <h2>Settings</h2>
    <v-row>
      <v-col sm="4">
        <v-btn @click="buildModel"> Build Face model </v-btn>

        <v-switch
          v-model="loadThumbData"
          color="primary"
          label="Load thumbnail data"
        ></v-switch
      ></v-col>
      <v-col sm="8">
        <h5>Media export profiles</h5>
        <v-list flat two-line>
          <v-list-item-group>
            <v-list-item
              v-for="profile in exportProfiles"
              :key="profile.id"
              @click="() => handleProfileClick(profile)"
            >
              <template v-slot:default>
                <v-list-item-action>
                  <v-checkbox :input-value="profile.isUserCurrent"></v-checkbox>
                </v-list-item-action>
                <v-list-item-content>
                  <v-list-item-title>{{ profile.name }}</v-list-item-title>

                  <div v-for="dest in profile.destinations" :key="dest.name">
                    <v-list-item-subtitle
                      >{{ dest.type }} -> {{ dest.name }}</v-list-item-subtitle
                    >
                  </div>
                </v-list-item-content>
              </template>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import { mapActions } from "vuex";

export default {
  created() {
    this.$store.dispatch("user/getExportProfiles");
  },
  computed: {
    loadThumbData: {
      set() {
        this.toggleLoadThumbnailData();
      },
      get() {
        return this.$store.state.media.loadThumbnailData;
      }
    },
    exportProfiles: function() {
      return this.$store.state.user.exportProfiles;
    }
  },
  methods: {
    handleProfileClick: function(profile) {
      this.$store.dispatch("user/updateCurrentExportProfile", profile);
    },
    ...mapActions("person", ["buildModel"]),
    ...mapActions("media", ["toggleLoadThumbnailData"]),
    ...mapActions("user", ["getExportProfiles", "updateCurrentExportProfile"])
  }
};
</script>

<style lang="scss" scoped></style>>
