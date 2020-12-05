<template>
  <v-card class="mx-auto" max-width="300">
    <v-toolbar color="blue" dark>
      <v-app-bar-nav-icon></v-app-bar-nav-icon>

      <v-toolbar-title>Inbox</v-toolbar-title>

      <v-spacer></v-spacer>

      <v-btn icon>
        <v-icon>mdi-magnify</v-icon>
      </v-btn>

      <v-btn icon>
        <v-icon>mdi-checkbox-marked-circle</v-icon>
      </v-btn>
    </v-toolbar>
    <v-list>
      <v-list-group prepend-icon="mdi-face-recognition">
        <template v-slot:activator>
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>Faces</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>

        <template v-for="item in settingsMenu.face.items">
          <v-list-item :key="item.title">
            <v-list-item-icon>
              <v-icon v-text="item.icon"></v-icon>
            </v-list-item-icon>
            <v-list-item-content>
              <v-list-item-title v-text="item.title"></v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list-group>
      <v-list-group prepend-icon="mdi-image-edit">
        <template v-slot:activator>
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>Actions</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>

        <template v-for="item in settingsMenu.actions.items">
          <v-list-item :key="item.title">
            <v-list-item-icon>
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
        <v-list-item-group v-model="settingsMenu.viewer.selected" multiple>
          <template v-for="item in settingsMenu.viewer.items">
            <v-list-item :key="item.title">
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
  </v-card>
</template>

<script>
export default {
  data() {
    return {
      settingsMenu: {
        viewer: {
          selected: [1],
          items: [
            { title: "Show faces boxes" },
            { title: "Show face list" },
            { title: "Show film stripe" },
            { title: "Show objects" },
          ],
        },
        face: {
          selected: [],
          items: [
            { title: "Approve all", icon: "mdi-check-all" },
            { title: "Unassign predicted", icon: "mdi-close-outline" },
            { title: "Remove unassigned", icon: "mdi-trash-can-outline" },
          ],
        },
        actions: {
          selected: [],
          items: [
            { title: "Move", icon: "mdi-file-move-outline" },
            { title: "Edit", icon: "mdi-pencil" },
            { title: "Delete", icon: "mdi-recycle" },
            { title: "Add to Album", icon: "mdi-plus" },
          ],
        },
      },
    };
  },
};
</script>

<style>
.player-container {
  height: 80vh;
  background-color: #000;
  font-weight: normal;
}
</style>