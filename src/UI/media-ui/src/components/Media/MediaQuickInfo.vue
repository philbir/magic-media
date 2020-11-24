<template>
  <v-container class="ma-0 pa-0">
    <v-row
      v-for="face in faces"
      :key="face.id"
      class="face-row"
      @click="editFace(face)"
      :style="{ 'border-left-color': faceColor(face) }"
    >
      <v-col cols="4" class="pa-0 ma-0">
        <img
          class="face-image"
          :src="'/api/media/thumbnail/' + face.thumbnail.id"
        />
      </v-col>
      <v-col class="pa-0">
        <v-row>
          <v-col sm="12" class="pa-0"> {{ faceTitle(face) }}</v-col>
        </v-row>
        <v-row>
          <v-col class="pa-0">
            <small>{{ age(face) }}</small>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import { getFaceColor } from "../../services/faceColor";

export default {
  props: {
    faces: Array,
  },
  data() {
    return {};
  },
  methods: {
    faceTitle: function (face) {
      let name = "...";
      if (face.person) {
        name = face.person.name;
      }
      return name;
    },
    age: function (face) {
      if (!face.age) {
        return "";
      } else if (face.age < 12) {
        return face.age + " months";
      } else {
        return Math.floor(face.age / 12) + " years";
      }
    },
    faceColor: function (face) {
      return getFaceColor(face);
    },
    editFace: function (face) {
      this.$store.dispatch("face/openEdit", face);
    },
  },
};
</script>

<style scoped>
.face-image {
  height: 60px;
  object-fit: contain;
  border-radius: 10px;
}
.face-row {
  height: 64px;
  color: #fff;
  width: 230px;
  border-left: #272727 solid 3px;
  margin-top: 4px;
  background-color: #272727;
}

.face-details {
}
</style>
