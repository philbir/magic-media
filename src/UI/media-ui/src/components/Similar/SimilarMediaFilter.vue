<template>
  <div>
    <v-sheet class="mx-4">
      <v-select
        :items="hashTypes"
        label="Hash Type"
        v-model="hashType"
        item-value="value"
        item-text="text"
      ></v-select>
      <v-text-field label="Similarity" v-model="similarity"></v-text-field>
    </v-sheet>

    <v-container style="height: 80vh; overflow-y: auto">
      <v-list>
        <v-list-item
          v-for="group in groups"
          :key="group.id"
          @click="onSelectGroup(group)"
        >
          <v-list-item-avatar size="56">
            <v-img
              :alt="group.id"
              :src="`/api/media/${group.id}/thumbnail/SqS`"
            ></v-img>
          </v-list-item-avatar>
          <v-list-item-content>
            <v-list-item-title v-text="group.count"></v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-container>
    <v-icon large @click="nextPage">mdi-arrow-right-bold-circle-outline</v-icon>
  </div>
</template>

<script>
import { mapActions, mapState } from "vuex";
export default {
  created() {
    this.getGroups();
  },
  data() {
    return {
      hashTypes: [
        { value: "FILE_HASH_SHA256", text: "File Hash" },
        { value: "IDENTIFIERS", text: "Media Identifiers" },
        { value: "IMAGE_AVERAGE_HASH", text: "Image Hash Average" },
        { value: "IMAGE_DIFFERENCE_HASH", text: "Image Hash Difference" },
        { value: "IMAGE_PERCEPTUAL_HASH", text: "Image Hash Perceptual" },
      ],
    };
  },
  computed: {
    ...mapState("similar", ["groups"]),
    hashType: {
      set(value) {
        this.setFilter({
          hashType: value,
        });
      },
      get() {
        return this.$store.state.similar.filter.hashType;
      },
    },
    similarity: {
      set(value) {
        this.setFilter({
          similarity: +value,
        });
      },
      get() {
        return this.$store.state.similar.filter.similarity;
      },
    },
  },
  methods: {
    ...mapActions("similar", [
      "getGroups",
      "selectGroup",
      "setFilter",
      "setPage",
    ]),
    onSelectGroup: function (group) {
      this.selectGroup(group);
    },
    nextPage: function () {
      this.setPage(+1);
    },
  },
};
</script>

<style>
</style>