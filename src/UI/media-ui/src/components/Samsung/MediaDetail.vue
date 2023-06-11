<template>
  <div>
    <div class="media-item">
      <v-img
        :src="`/api/samsung/thumbnail/FrameDining/${$route.params.id}`"
        width="500"
        class="polar"
      />
    </div>
    <v-card flat>
      <v-card-text>
        <v-form>
          <v-row>
            <v-col cols="12" md="6">
              <v-select
                v-model="matteId"
                :items="mattes"
                label="Matte"
                @change="onChangeMatte"
              ></v-select>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" md="6">
              <v-select
                v-model="filterId"
                :items="filters"
                label="Filter"
                @change="onChangeFilter"
              ></v-select>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import { mapActions } from "vuex";

//import { mapActions, mapState } from "vuex";
export default {
  data() {
    return {
      media: {},
      matteId: null,
      filterId: null
    };
  },
  created() {
    if (this.mattes.length == 0) this.$store.dispatch("samsung/getFeatures");
  },
  props: {
    id: String
  },
  methods: {
    ...mapActions("samsung", ["changeMatte", "changeFilter"]),
    onChangeMatte: function() {
      console.log("onChangeMatte", this.$route.params.id, this.matteId);
      this.changeMatte({ id: this.$route.params.id, matte: this.matteId });
    },
    onChangeFilter: function() {
      this.changeFilter(this.id, this.matteId);
    }
  },
  computed: {
    mattes: function() {
      return this.$store.state.samsung.features.mattes;
    },
    filters: function() {
      return this.$store.state.samsung.features.filters;
    }
  }
};
</script>

<style scoped>
.media-item {
  margin-top: 20px;
  position: relative;
}

/* a style which look like a wide image frame */
.polar {
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  margin: auto;
  background: #fff;
  padding: 30px;
  border-style: solid;
  border-width: 15px;
  border-top-color: lighten(#000, 20%);
  border-right-color: lighten(#000, 0%);
  border-bottom-color: lighten(#000, 20%);
  border-left-color: lighten(#000, 0%);
  box-shadow: 2px 2px 4px rgba(0, 0, 0, 0.6);
}
</style>
