<template>
  <div v-if="face && box">
    <div
      v-if="image.loaded"
      class="face-box"
      @click="this.editFace"
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
        {{ face.person.name }}
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable no-debugger */

import { getFaceColor } from "../services/faceColor";

export default {
  mounted() {
    this.$nextTick(() => {});
  },
  props: ["face", "image"],
  data() {
    return {};
  },
  computed: {
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
    box: function () {
      const box = {};

      const ratio = this.image.naturalWidth / this.image.width;
      box.left = Math.round(this.face.box.left / ratio) + this.image.offsetLeft;
      box.top = Math.round(this.face.box.top / ratio) + this.image.offsetTop;
      box.width = Math.round(
        (this.face.box.right - this.face.box.left) / ratio
      );
      box.height = Math.round(
        (this.face.box.bottom - this.face.box.top) / ratio
      );
      box.showName = box.width > 30 && this.face.person;
      if (box.showName) {
        box.height = box.height + 12;
        box.top = box.top - 12;
      }
      box.color = getFaceColor(this.face);

      return box;
    },
  },
  methods: {
    editFace() {
      this.$store.dispatch("face/openEdit", this.face);
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
  z-index: 101;
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
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}
</style>
