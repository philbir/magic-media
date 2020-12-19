<template>
  <div class="container" v-if="medias.length > 0">
    <img
      class="thumb"
      v-for="id in medias"
      :key="id"
      :src="`/api/media/${id}/thumbnail/S`"
      :ref="'img' + id"
      @click="onClick(id)"
    />
  </div>
  <v-progress-linear v-else indeterminate></v-progress-linear>
</template>

<script>
export default {
  props: {
    ids: Array,
  },
  data() {
    return {};
  },
  computed: {
    medias: function () {
      return this.ids;
    },
  },
  methods: {
    onClick: function (id) {
      this.$emit("select", id);
    },
  },
};
</script>


<style scoped>
.container {
  display: block;
  padding: 0;
  white-space: nowrap;
  overflow-x: auto;
  transition: all 0.2s ease-in-out;
  transition-property: all;
  transition-duration: 0.2s;
  transition-timing-function: ease-in-out;
  transition-delay: 0s;
}

.thumb {
  height: 65px;
  width: 65px;
  object-fit: cover;
  border-radius: 10px;
  margin-left: 10px;
  opacity: 0.75;
}

.thumb:hover {
  opacity: 1;
}

.selected {
  border: 2px solid white;
}

::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track:hover {
  background: #b9dff8;
}
::-webkit-scrollbar-track:active {
  background: #dcdfe0;
}

::-webkit-scrollbar-track {
  background: #e3eaf0;
  border: 0px none #ffffff;
  border-radius: 50px;
}

::-webkit-scrollbar-thumb {
  background: #68bef881;
  border: 0px none #ffffff;
  border-radius: 50px;
}
::-webkit-scrollbar-thumb:hover {
  background: #49a3df;
}
::-webkit-scrollbar-thumb:active {
  background: #3481b4;
}
</style>
