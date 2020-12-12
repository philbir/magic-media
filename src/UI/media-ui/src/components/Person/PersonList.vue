<template>
  <div>
    <v-container>
      <v-row style="overflow-x: auto; height: 92vh">
        <v-col v-for="person in persons" :key="person.id" sm="3" lg="4">
          <v-card width="400" min-height="200">
            <v-card-title class="font-weight-bold">
              <v-avatar size="56">
                <img alt="user" :src="thumbnail(person)" />
              </v-avatar>
              <p class="ml-3">
                {{ person.name }}
              </p>
              <v-spacer></v-spacer>
              <p>{{ person.summary.mediaCount.toLocaleString() }}</p>
            </v-card-title>
            <v-card-text>
              <v-row>
                <v-col>
                  <v-icon v-if="person.dateOfBirth"> mdi-cake-variant </v-icon>
                  <strong>
                    {{ person.dateOfBirth | dateformat("DATE_SHORT") }}</strong
                  >
                </v-col>
                <v-spacer></v-spacer>
                <v-col>
                  To validate:
                  <strong>{{
                    person.summary.mediaCount - person.summary.validatedCount
                  }}</strong>
                </v-col>
              </v-row>

              <v-chip-group multiple>
                <v-chip
                  v-for="group in person.groups"
                  small
                  :key="group.id"
                  v-text="group.name"
                  color="blue lighten-4"
                >
                </v-chip>
              </v-chip-group>
            </v-card-text>

            <v-card-actions>
              <v-btn outlined rounded icon @click="openTimeline(person)">
                <v-icon> mdi-timeline-clock-outline </v-icon>
              </v-btn>
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
import EditPersonDialog from "./EditPersonDialog";

export default {
  components: {
    EditPersonDialog,
  },
  created() {
    this.$store.dispatch("person/search");
  },
  data() {
    return {
      showEditDialog: false,
      editPersonId: null,
    };
  },
  computed: {
    persons: function () {
      return this.$store.state.person.list;
    },
  },
  methods: {
    thumbnail: function (person) {
      if (person.thumbnail) {
        return person.thumbnail.dataUrl;
      } else {
        return null;
      }
    },
    editClick: function (id) {
      this.editPersonId = id;
      this.showEditDialog = true;
    },
    openTimeline: function (person) {
      this.$router.push({ name: "PersonTimeline", params: { id: person.id } });
    },
  },
};
</script>

<style>
</style>