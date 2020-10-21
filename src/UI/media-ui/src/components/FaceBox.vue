<template>
  <div>
    <div
      v-if="box"
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
                :src="
                  'http://localhost:5000/media/thumbnail/' +
                  faceData.thumbnail.id
                "
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
                  :value="faceData.person ? faceData.person.name : null"
                  :items="['Yael', 'Jana', 'Philippe', 'Eun Ju']"
                  label="Name"
                  v-on:change="setName"
                ></v-combobox>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-btn
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
import MUTATION_ASSIGN_PERSON from "../graphql/AssignPersonByHuman.gql";
import { getFaceColor } from "../services/faceColor";

export default {
  created() {
    this.faceData = this.face;
  },
  mounted() {
    this.$nextTick(() => {
      window.setTimeout(() => {
        // img.naturalWidth is 0 without this timeout
        const box = {};
        const img = this.$parent.$refs.img;
        const ratio = img.naturalWidth / img.width;
        console.log(img.offsetLeft, img.offsetTop, img.naturalWidth, img.width);
        box.left = Math.round(this.face.box.left / ratio) + img.offsetLeft;
        box.top = Math.round(this.face.box.top / ratio) + img.offsetTop;
        box.width = Math.round(
          (this.face.box.right - this.face.box.left) / ratio
        );
        box.height = Math.round(
          (this.face.box.bottom - this.face.box.top) / ratio
        );
        this.setBoxProperties(box, this.face);
        this.setData(box);
      }, 50);
    });
  },
  props: ["face"],
  data() {
    return {
      box: null,
      dialog: false,
      faceData: null,
    };
  },
  methods: {
    setData(box) {
      console.log(box);
      this.box = box;
    },
    accept() {},
    async setName(name) {
      const result = await this.$apollo.mutate({
        mutation: MUTATION_ASSIGN_PERSON,
        variables: {
          input: {
            faceId: this.face.id,
            personName: name,
          },
        },
      });
      this.faceData = result.data.assignPersonByHuman.face;
      this.setBoxProperties(this.box, this.faceData);
      console.log("UPD FACE", result.data.assignPersonByHuman);
      this.dialog = false;
    },
    setBoxProperties(box, face) {
      box.showName = box.width > 30 && face.person;
      if (box.showName) {
        box.height = box.height + 12;
        box.top = box.top - 12;
      }
      box.color = getFaceColor(face);
    },
    openDialog() {
      this.dialog = true;
      this.$nextTick(() => {
        this.setFocus();
      });
    },
    setFocus() {
      console.log(this.$refs.combo.$refs.input);
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