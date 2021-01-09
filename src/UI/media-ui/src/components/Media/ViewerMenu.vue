<template>
  <v-menu :close-on-content-click="false" open-on-hover bottom offset-y>
    <template v-slot:activator="{ on, attrs }">
      <v-icon v-bind="attrs" v-on="on" class="mr-0" color="white">
        mdi-cog-outline
      </v-icon>
    </template>

    <v-list>
      <v-list-group
        v-if="userActions.media.edit"
        prepend-icon="mdi-face-recognition"
      >
        <template v-slot:activator>
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>Faces</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>

        <template v-for="item in menu.face.items">
          <v-list-item :key="item.title" @click="onFaceActionClick(item.value)">
            <v-list-item-icon>
              <v-icon v-text="item.icon"></v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title v-text="item.title"></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list-group>
      <v-list-group v-if="userActions.media.edit" prepend-icon="mdi-image-edit">
        <template v-slot:activator>
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>Actions</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>

        <template v-for="item in menu.actions.items">
          <v-list-item :key="item.title" @change="onActionClick(item.action)">
            <v-list-item-icon>
              <v-icon v-text="item.icon"></v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title v-text="item.title"></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list-group>

      <v-list-group
        v-if="userActions.media.download"
        prepend-icon="mdi-file-download"
      >
        <template v-slot:activator>
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>Download</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>

        <template v-for="item in downloadMenu">
          <v-list-item :key="item.title" @change="onDownloadClick(item.action)">
            <v-list-item-icon v-if="item.icon">
              <v-icon v-text="item.icon"></v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title v-text="item.title"></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list-group>

      <v-list-group prepend-icon="mdi-eye-settings-outline">
        <template v-slot:activator>
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>View settings</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
        <v-list-item-group
          v-model="menu.viewer.selected"
          @change="onViewOptionsChange"
          multiple
        >
          <template v-for="item in menu.viewer.items">
            <v-list-item :key="item.title" :value="item.value">
              <template v-slot:default="{ active }">
                <v-list-item-content>
                  <v-list-item-title v-text="item.title"></v-list-item-title>
                </v-list-item-content>

                <v-list-item-action>
                  <v-list-item-action-text
                    v-text="item.action"
                  ></v-list-item-action-text>

                  <v-switch :input-value="active"></v-switch>
                </v-list-item-action>
              </template>
            </v-list-item>
          </template>
        </v-list-item-group>
      </v-list-group>
    </v-list>
  </v-menu>
</template>

<script>
import { mapGetters, mapState } from "vuex";
export default {
  created() {
    this.setViewOptionsSelected(this.viewerOptions);
  },
  watch: {
    viewerOptions: function (newValue) {
      this.setViewOptionsSelected(newValue);
    },
  },
  data() {
    return {
      menu: {
        viewer: {
          selected: ["SHOW_FACE_LIST"],
          items: [
            { title: "Face boxes", value: "SHOW_FACE_BOXES" },
            { title: "Face list", value: "SHOW_FACE_LIST" },
            { title: "Film stripe", value: "SHOW_FILM_STRIPE" },
            { title: "Objects", value: "SHOW_OBJECTS" },
          ],
        },
        face: {
          selected: [],
          items: [
            {
              title: "Approve all [a]",
              icon: "mdi-check-all",
              value: "APPROVE_ALL",
            },
            {
              title: "Predict [p]",
              icon: "mdi-auto-fix",
              value: "PREDICT",
            },
            {
              title: "Unassign predicted [u]",
              icon: "mdi-close-outline",
              value: "UNASIGN_PREDICTED",
            },
            {
              title: "Remove unassigned  [r]",
              icon: "mdi-trash-can-outline",
              value: "DELETED_UNASSINGNED",
            },
          ],
        },
        actions: {
          selected: [],
          items: [
            { title: "Move", icon: "mdi-file-move-outline", action: "MOVE" },
            { title: "Edit", icon: "mdi-pencil", action: "EDIT" },
            {
              title: "Analyse CloudAI",
              action: "AI",
              icon: "mdi-cloud-check-outline",
            },
            { title: "Delete", icon: "mdi-recycle", action: "RECYCLE" },
            { title: "Add to Album", icon: "mdi-plus", action: "ADD_TO_ALBUM" },
          ],
        },
      },
    };
  },
  computed: {
    ...mapState("media", { media: "current", viewerOptions: "viewer" }),
    ...mapGetters("user", ["userActions"]),
    downloadMenu: function () {
      const items = [];

      const icon =
        this.media.mediaType === "IMAGE"
          ? "mdi-file-image"
          : "mdi-file-video-outline";

      if (this.media) {
        items.push({
          title: `Original: ${this.media.dimension.width} x ${
            this.media.dimension.height
          } (${this.formatSize(this.media.size)})`,
          action: "ORIGINAL",
          icon,
        });

        if (this.media.mediaType === "IMAGE") {
          items.push({
            title: "Medium",
            action: "MEDIUM",
            icon,
          });

          items.push({
            title: "Small",
            action: "SMALL",
            icon,
          });

          items.push({
            title: "With advanced option",
            action: "ADVANCED",
            icon,
          });
        } else {
          items.push({
            title: "720P",
            action: "720P",
            icon,
          });
        }
      }

      return items;
    },
  },
  methods: {
    formatSize: function (size) {
      return (size / 1024.0 / 1024.0).toPrecision(2) + " MB";
    },
    onFaceActionClick: function (action) {
      this.$emit("faceAction", action);
    },
    onActionClick: function (action) {
      this.$emit("mediaAction", action);
    },
    onDownloadClick: function (action) {
      console.log(action);
      location.href = `/api/download/${this.media.id}`;
    },
    setViewOptionsSelected: function (options) {
      var selected = [];
      if (options.showFaceBox) {
        selected.push("SHOW_FACE_BOXES");
      }
      if (options.showFaceList) {
        selected.push("SHOW_FACE_LIST");
      }
      if (options.showFilmStripe) {
        selected.push("SHOW_FILM_STRIPE");
      }
      if (options.showObjects) {
        selected.push("SHOW_OBJECTS");
      }

      this.menu.viewer.selected = selected;
    },
    onViewOptionsChange: function () {
      var options = {
        showFaceBox: this.menu.viewer.selected.includes("SHOW_FACE_BOXES"),
        showFaceList: this.menu.viewer.selected.includes("SHOW_FACE_LIST"),
        showFilmStripe: this.menu.viewer.selected.includes("SHOW_FILM_STRIPE"),
        showObjects: this.menu.viewer.selected.includes("SHOW_OBJECTS"),
      };

      this.$store.dispatch("media/setViewerOptions", options);
    },
  },
};
</script>

<style>
</style>