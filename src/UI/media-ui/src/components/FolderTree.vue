<template>
  <div>
    <v-row>
      <v-btn class="ml-1" color="blue lighten-4" small>
        <v-icon> mdi-folder-outline </v-icon>
      </v-btn>
      <v-btn class="ml-1" color="gray" small>
        <v-icon> mdi-earth</v-icon>
      </v-btn>
      <v-btn class="ml-1" color="gray" small>
        <v-icon> mdi-account-outline</v-icon>
      </v-btn>
      <v-btn class="ml-1" color="gray" small>
        <v-icon> mdi-calendar-range-outline</v-icon>
      </v-btn>
      <v-btn class="ml-1" color="gray" small>
        <v-icon> mdi-dots-horizontal</v-icon>
      </v-btn>
    </v-row>

    <v-treeview
      :open="initiallyOpen"
      :items="items"
      dense
      activatable
      item-key="name"
      open-on-click
    >
      <template v-slot:prepend="{ open }">
        <v-icon color="blue">
          {{ open ? "mdi-folder-open" : "mdi-folder" }}
        </v-icon>
      </template>
    </v-treeview>
  </div>
</template>

<script>
export default {
  data: () => ({
    initiallyOpen: ["Home"],
    files: {},
    tree: [],
    items: [
      {
        name: "Home",
        children: [
          {
            name: "Family",
            children: [
              {
                name: "2020",
              },
              {
                name: "2019",
              },
            ],
          },
          {
            name: "Vacation",
          },
          {
            name: "Kids",
          },
        ],
      },
    ],
  }),
};
</script>

<style>
</style>