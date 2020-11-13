<template>
  <div>
    <v-container>
      <v-row style="overflow-x: auto; height: 600px">
        <v-col v-for="person in persons" :key="person.id" sm="4" lg="6">
          <v-card width="400">
            <v-card-title class="font-weight-bold">
              <v-avatar size="56">
                <img alt="user" :src="`/api/person/thumbnail/${person.id}`" />
              </v-avatar>
              <p class="ml-3">
                {{ person.name }}
              </p>
            </v-card-title>

            <v-card-text> </v-card-text>

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
    persons: function () {
      console.log(this.$store.state.person.persons);
      return this.$store.state.person.persons;
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
