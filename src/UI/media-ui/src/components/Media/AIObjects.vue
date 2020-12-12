<template>
  <div>
    <div
      v-for="(obj, i) in objectData"
      :key="i"
      class="object-box"
      :title="obj.title"
      :style="{
        left: obj.bbox.left + 'px',
        top: obj.bbox.top + 'px',
        height: obj.bbox.height + 'px',
        width: obj.bbox.width + 'px',
        'border-color': obj.bbox.color,
      }"
    >
      <div
        v-if="obj.bbox.showTitle"
        class="box-title"
        :style="{ 'background-color': obj.bbox.color }"
      >
        {{ obj.title }}
      </div>
    </div>
  </div>
</template>

<script>
import { getCssBox } from "../../services/imageBoxUtils";

export default {
  props: ["objects", "image"],
  computed: {
    objectData: function () {
      return this.objects.map((x) => {
        x.bbox = this.getbox(x);
        x.title = `${x.name} ${x.confidence.toPrecision(3)}%`;

        return x;
      });
    },
  },
  methods: {
    getbox: function (item) {
      const box = getCssBox(item.box, this.image, 12);

      if (item.source === "AZURE_C_V") {
        box.color = "#d900b8";
      } else {
        box.color = "#a600d9";
      }

      return box;
    },
  },
};
</script>

<style scoped>
.object-box {
  position: absolute;
  border-width: 1 px;
  border-style: solid;
  z-index: 99;
}

.box-title {
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