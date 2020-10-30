<template>
  <div v-if="faceData && box">
    <div
      v-if="image.loaded"
      class="face-box"
      @click="this.openDialog"
      :style="{
        left: box.left + 'px',
        top: box.top + 'px',
        height: box.height + 'px',
        width: box.width + 'px',
        'border-color': box.color,
      }"
    >
      <div
        v-if="box.showName"
        class="face-name"
        :style="{ 'background-color': box.color }"
      >
        {{ faceData.person.name }}
      </div>
    </div>

    <v-dialog v-model="dialog" width="400">
      <v-card elevation="2" outlined>
        <v-card-title>
          <v-row dense>
            <v-col sm="9">
              <span>
                {{
                  faceData.person ? faceData.person.name : "Unknown person"
                }}</span
              ></v-col
            >
            <v-spacer></v-spacer>
            <v-col sm="3">
              <img
                :src="'/api/media/thumbnail/' + faceData.thumbnail.id"
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
                  :value="faceData.person ? faceData.person.name : null"
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
  </div>
</template>

<script>
/* eslint-disable no-debugger */
import {
  assignPerson,
  unAssignPerson,
  deleteFace,
} from "../services/faceService";
import { getFaceColor } from "../services/faceColor";

export default {
  created() {
    this.faceData = this.face;
  },
  mounted() {
    this.$nextTick(() => {});
  },
  props: ["face", "image"],
  data() {
    return {
      dialog: false,
      faceData: this.face,
    };
  },
  computed: {
    personNames: function () {
      return this.$store.state.persons.map((item) => item.name);
    },
    showApproved: function () {
      return (
        this.faceData.recognitionType === "COMPUTER" &&
        this.faceData.state !== "VALIDATED"
      );
    },
    showUnassign: function () {
      return this.faceData.person !== null;
    },
    box: function () {
      const box = {};

      const ratio = this.image.naturalWidth / this.image.width;
      box.left =
        Math.round(this.faceData.box.left / ratio) + this.image.offsetLeft;
      box.top =
        Math.round(this.faceData.box.top / ratio) + this.image.offsetTop;
      box.width = Math.round(
        (this.faceData.box.right - this.faceData.box.left) / ratio
      );
      box.height = Math.round(
        (this.faceData.box.bottom - this.faceData.box.top) / ratio
      );
      box.showName = box.width > 30 && this.faceData.person;
      if (box.showName) {
        box.height = box.height + 12;
        box.top = box.top - 12;
      }
      box.color = getFaceColor(this.faceData);
      console.log("COMPUTED_BOX", box);

      return box;
    },
  },
  methods: {
    accept() {},
    async setName(name) {
      if (name) {
        const result = await assignPerson(this.faceData.id, name);
        console.log(result.data.assignPersonByHuman.face);
        this.faceData = result.data.assignPersonByHuman.face;
        this.$store.commit("PERSON_ADDED", this.faceData.person);

        this.dialog = false;
      }
    },
    async unAssignPerson() {
      const result = await unAssignPerson(this.faceData.id);
      this.faceData = result.data.unAssignPersonFromFace.face;
    },
    async deleteFace() {
      const result = await deleteFace(this.faceData.id);
      console.log(result);
      this.faceData = null;
    },
    openDialog() {
      this.dialog = true;
      this.$nextTick(() => {
        this.setFocus();
      });
    },
    setFocus() {
      if (this.faceData.person === null) this.$refs.combo.$refs.input.focus();
    },
  },
};
</script>

<style scoped>
.face-box {
  position: absolute;
  border-color: blueviolet;
  border-width: 1 px;
  border-style: solid;
}

.face-name {
  color: #fff;
  font-size: 12px;
  font-weight: 300;
  line-height: 14px;
  text-align: center;
  position: absolute;
  width: 100%;
  height: 14px;
}
.dialog-face-image {
  height: 74px;
  width: 74px;
  border-radius: 100%;
}
</style>
