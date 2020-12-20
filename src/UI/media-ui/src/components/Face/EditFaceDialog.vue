<template>
  <v-dialog v-model="dialog" width="400">
    <v-card elevation="2" outlined :loading="inProgress" v-if="face != null">
      <v-card-title>
        <v-row dense>
          <v-col sm="9">
            <span> {{ faceTitle }}</span></v-col
          >
          <v-spacer></v-spacer>
          <v-col sm="3">
            <img
              :src="`/api/face/${face.id}/thumbnail/${face.thumbnail.id}`"
              class="dialog-face-image"
            />
          </v-col>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-container>
          <v-row dense>
            <v-col cols="12" sm="12">
              <v-combobox
                ref="combo"
                dense
                clearable
                :value="face.person ? face.person.name : null"
                :items="personNames"
                label="Name"
                v-on:change="setName"
              ></v-combobox>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-btn
                v-show="showApproved"
                @click="approve"
                depressed
                elevation="2"
                large
                color="green"
                class="ma-1"
                icon
              >
                <v-icon dark> mdi-emoticon-happy </v-icon></v-btn
              >
              <v-btn
                v-show="showUnassign"
                @click="unAssignPerson"
                depressed
                elevation="2"
                large
                color="red"
                class="ma-1"
                icon
              >
                <v-icon dark> mdi-emoticon-sad </v-icon></v-btn
              >
              <v-btn
                @click="deleteFace"
                depressed
                elevation="2"
                large
                color="grey"
                class="ma-1"
                icon
              >
                <v-icon dark> mdi-trash-can</v-icon></v-btn
              >
              <v-btn
                v-if="showPredict"
                @click="predictPerson"
                depressed
                elevation="2"
                large
                color="blue"
                class="ma-1"
                icon
              >
                <v-icon dark> mdi-auto-fix</v-icon></v-btn
              >
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="dialog = false">
          Close
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import { mapGetters } from "vuex";
export default {
  created() {
    this.faceData = this.face;
  },
  mounted() {
    this.$nextTick(() => {});
  },
  data() {
    return {
      dialog: false,
      inProgress: false,
    };
  },
  watch: {
    faceId: function (newValue) {
      if (newValue && this.userActions.face.edit) {
        this.dialog = true;
      }
    },
    dialog: function (newValue) {
      if (newValue === false) {
        this.$store.dispatch("face/closeEdit");
      }
    },
  },
  computed: {
    ...mapGetters("user", ["userActions"]),
    personNames: function () {
      return this.$store.state.person.persons.map((item) => item.name);
    },
    faceTitle: function () {
      let name = "Unknown";
      if (this.face.person) {
        name = this.face.person.name;

        if (this.face.age) {
          name += ` (${Math.floor(this.face.age / 12)})`;
        }
      }
      return name;
    },
    faceId: function () {
      return this.$store.state.face.editFaceId;
    },
    face: function () {
      return this.$store.state.face.editFace;
    },
    showApproved: function () {
      return (
        this.face.recognitionType === "COMPUTER" &&
        this.face.state !== "VALIDATED"
      );
    },
    showUnassign: function () {
      return this.face.person !== null;
    },
    showPredict: function () {
      return this.face.person === null;
    },
  },
  methods: {
    async setName(name) {
      if (name) {
        this.$store.dispatch("face/setName", { id: this.face.id, name: name });
        this.dialog = false;
      }
    },
    async unAssignPerson() {
      this.$store.dispatch("face/unassignPerson", this.face.id);
      this.dialog = false;
    },
    async approve() {
      this.$store.dispatch("face/approve", this.face.id);
      this.dialog = false;
    },
    async deleteFace() {
      this.$store.dispatch("face/deleteFace", this.face).then(() => {
        this.$magic.snack("Face deleted", "SUCCESS");
      });
      this.dialog = false;
    },
    async predictPerson() {
      this.inProgress = true;
      this.$store.dispatch("face/predictPerson", this.face.id);
      this.inProgress = false;
    },
  },
};
</script>

<style scoped>
.dialog-face-image {
  height: 74px;
  width: 74px;
  border-radius: 100%;
}
</style>
