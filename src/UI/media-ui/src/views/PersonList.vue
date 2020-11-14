<template>
  <div>
    <v-container>
      <v-row style="overflow-x: auto; height: 600px">
        <v-col v-for="person in persons" :key="person.id" sm="3" lg="4">
          <v-card width="400">
            <v-card-title class="font-weight-bold">
              <v-avatar size="56">
                <img alt="user" :src="`/api/person/thumbnail/${person.id}`" />
              </v-avatar>
              <p class="ml-3">
                {{ person.name }}
              </p>
            </v-card-title>

            <v-card-text>
              <p>
                {{ person.dateOfBirth | dateformat("DATE_SHORT") }}
              </p>
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>

              <v-btn outlined rounded text @click="editClick(person.id)">
                Edit
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
    <EditPersonDialog
      :personId="editPersonId"
      :show="showEditDialog"
      @close="
        showEditDialog = false;
        editPersonId = null;
      "
    />
  </div>
</template>

<script>
import EditPersonDialog from "../components/EditPersonDialog";
/* eslint-disable no-debugger */

export default {
  components: {
    EditPersonDialog,
  },

  data() {
    return {
      showEditDialog: false,
      editPersonId: null,
    };
  },
  computed: {
    filters: function () {
      return this.$store.state.person.filter;
    },
    persons: function () {
      return this.$store.state.person.persons.filter((x) => {
        if (this.filters.searchText === "") {
          return true;
        } else {
          return x.name
            .toLowerCase()
            .includes(this.filters.searchText.toLowerCase());
        }
      });
    },
  },
  methods: {
    editClick: function (id) {
      this.editPersonId = id;
      this.showEditDialog = true;
    },
  },
};
</script>

<style>
</style>
