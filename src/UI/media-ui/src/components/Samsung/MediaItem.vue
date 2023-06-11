<template>
  <div
    class="media-item"
    @mouseover="isActive = true"
    @mouseout="isActive = false"
    @click="onClick(media)"
    :class="{ selected: media.selected }"
  >
    <v-img
      :src="`/api/samsung/thumbnail/FrameDining/${media.id}`"
      height="140"
      width="248"
      class="polar"
    />
    <div class="media-footer" :class="{ active: isActive }">
      <small>{{ media.id }} </small>
      <v-btn fab dark x-small color="blue" class="mx-1" @click="onClickDetails">
        <v-icon dark>
          mdi-image-frame
        </v-icon>
      </v-btn>
      <v-btn fab dark x-small color="red" @click="onClickDelete">
        <v-icon dark>
          mdi-trash-can-outline
        </v-icon>
      </v-btn>
    </div>
  </div>
</template>

<script>
import { mapActions } from "vuex";

//import { mapActions, mapState } from "vuex";
export default {
  data() {
    return {
      isActive: false
    };
  },
  props: {
    media: Object
  },
  methods: {
    ...mapActions("samsung", ["selectMedia", "deleteMedia"]),
    onClick() {
      this.selectMedia(this.media.id);
    },
    onClickDelete() {
      this.deleteMedia(this.media.id);
    },
    onClickDetails() {
      this.$router.push({
        name: "SamsungTvMedia",
        params: { id: this.media.id }
      });
    }
  }
};
</script>

<style scoped>
.media-item {
  position: relative;
}

.media-footer {
  bottom: 0px;
  width: 100%;
  height: 40px;
  background-color: #504f4f;
  opacity: 0;
  position: absolute;
  color: #fff;
  font-weight: bolder;
  padding: 6px;
  font-size: 1em;
  border-radius: 6px;
}

.media-footer.active {
  opacity: 0.8;
  transition: opacity;
  transition-timing-function: ease-in-out;
  transition-duration: 400ms;
  transition-delay: 0s;
}

.media-item.selected {
  border-left: rgb(195, 0, 206) 6px solid;
}

/* a style which look like a wide image frame */
.polar {
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  margin: auto;
  padding: 30px;
  border-style: solid;
  border-width: 15px;
  border-top-color: lighten(#6b6b6b, 20%);
  border-right-color: lighten(#8c8b8b, 0%);
  border-bottom-color: lighten(#878686, 20%);
  border-left-color: lighten(#6f6f6f, 0%);
  box-shadow: 2px 2px 4px rgba(117, 117, 117, 0.6);
}
</style>
