<template>
  <v-progress-linear v-if="!person" indeterminate color="blue" top />

  <div v-else style="overflow-x: auto; height: 92vh">
    <v-row class="ma-0 pa-0">
      <v-col class="ma-0 pa-2" sm="1">
        <v-icon @click="back" class="ml-6" color="grey" large>
          mdi-arrow-left-circle
        </v-icon>
      </v-col>
      <v-col sm="11">
        <h2 class="ml-4 grey--text">Timeline {{ person.name }}</h2>
      </v-col>
    </v-row>

    <v-timeline dense>
      <v-timeline-item
        v-for="age in person.timeline.ages"
        :key="age.age"
        style="posistion: absolute"
        large
      >
        <template v-slot:icon>
          <h3 class="white--text">{{ age.age }}</h3>
        </template>
        <div class="face-container">
          <div
            v-for="face in age.faces"
            :key="face.id"
            v-on:click="clickFace(face, $event)"
          >
            <img class="face-image" :src="face.thumbnail.dataUrl" />
          </div>
        </div>
      </v-timeline-item>
    </v-timeline>
  </div>
</template>

<script>
import { getTimeline } from "../../services/personService";

export default {
  created() {
    this.load(this.personId);
  },
  data() {
    return {
      person: null,
    };
  },
  computed: {
    personId: function () {
      return this.$route.params.id;
    },
  },
  methods: {
    async load(id) {
      var res = await getTimeline(id);
      this.person = res.data.person;
    },
    clickFace: function (face, e) {
      if (e.ctrlKey) {
        this.$store.dispatch("face/openEdit", face);
      } else {
        this.$store.dispatch("media/show", face.mediaId);
      }
    },
    back: function () {
      this.$router.push({ name: "Persons" });
    },
  },
};
</script>

<style lang="sass" scoped>
.face-container
  position: relative
  display: flex
  align-items: flex-start
  align-content: space-around

.face-image
  margin-right: 10px
  border-radius: 100%
  height: 120px
  width: 120px
</style>