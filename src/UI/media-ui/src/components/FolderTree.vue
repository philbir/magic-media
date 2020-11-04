<template>
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