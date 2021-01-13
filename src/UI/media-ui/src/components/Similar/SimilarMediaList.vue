<template>
  <div>
    <v-progress-linear v-if="loading" indeterminate color="blue" top />
    <v-container style="height: 90vh; overflow-y: auto" v-if="selectedGroup">
      <v-sheet
        rounded="lg"
        elevation="2"
        v-for="media in selectedGroup.medias"
        :key="media.id"
        class="ma-2"
      >
        <v-row @click="onClickMedia(media.id)">
          <v-col sm="4">
            <img
              class="image ml-4"
              :alt="media.filename"
              :src="`/api/media/${media.id}/thumbnail/M`"
            />
          </v-col>
          <v-col sm="8">
            <div>
              <v-row class="ma-0 pa-0">
                <v-col sm="2">Date Taken</v-col>
                <v-col sm="10" class="font-weight-bold">
                  {{ media.dateTaken | dateformat("DATETIME_MED") }}
                </v-col>
              </v-row>
              <v-row class="ma-0 pa-0">
                <v-col sm="2">Folder</v-col>
                <v-col sm="10" class="font-weight-bold">
                  {{ media.folder }}
                </v-col>
              </v-row>
              <v-row class="ma-0 pa-0">
                <v-col sm="2">Filename</v-col>
                <v-col sm="10" class="font-weight-bold">
                  {{ media.filename }}
                </v-col>
              </v-row>
            </div>
          </v-col>
        </v-row>
      </v-sheet>
    </v-container>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
export default {
  data() {
    return {};
  },
  computed: {
    ...mapState("similar", ["groups", "selectedGroupId", "loading"]),
    selectedGroup: function () {
      if (this.selectedGroupId) {
        return this.groups.filter((x) => x.id == this.selectedGroupId)[0];
      }

      return null;
    },
  },
  methods: {
    ...mapActions("similar", ["getGroups"]),
    onClickMedia: function (id) {
      this.$store.dispatch("media/show", id);
    },
  },
};
</script>

<style scoped>
.container {
  display: block;
}

.image {
  height: 180px;
  border-radius: 10px;
}
</style>



