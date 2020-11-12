<template>
  <v-dialog width="500" v-model="isOpen">
    <v-card elevation="2" v-if="person">
      <v-card-title> Edit {{ person.name }}</v-card-title>
      <v-card-text>
        <v-form v-model="valid">
          <v-container>
            <v-row>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model="person.name"
                  label="Name"
                  required
                ></v-text-field>
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field
                  v-model="person.dateOfBirth"
                  v-mask="'##.##.####'"
                  label="Date of Birth"
                  required
                ></v-text-field
              ></v-col>

              <v-col cols="12" md="12">
                <v-combobox
                  v-model="person.groups"
                  :items="groups"
                  :search-input.sync="groupSearch"
                  hide-selected
                  label="Groups"
                  chips
                  multiple
                  clearable
                  deletable-chips
                >
                  <template v-slot:no-data>
                    <v-list-item v-show="groupSearch">
                      <v-list-item-content>
                        <v-list-item-title>
                          Press enter to create new group:
                          <strong>{{ groupSearch }}</strong>
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </template>
                </v-combobox>
              </v-col>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>
        <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
        <v-btn color="primary" text @click="save"> Save </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
export default {
  props: {
    show: {
      type: Boolean,
    },
    personId: {
      type: String,
    },
  },
  data() {
    return {
      person: {},
      valid: true,
      groups: ["Family"],
      groupSearch: "",
    };
  },
  watch: {
    personId: function (newValue) {
      if (newValue) {
        const person = this.$store.state.person.persons.find(
          (x) => x.id === newValue
        );

        this.person = { ...person };
      } else {
        this.person = {};
      }
    },
  },
  computed: {
    isOpen: {
      get() {
        return this.show;
      },
      set(val) {
        this.$emit("close", val);
      },
    },
  },
  methods: {
    save: function () {
      console.log(this.person);

      this.isOpen = false;
    },
    cancel: function () {
      this.isOpen = false;
    },
  },
};
</script>

<style scoped>
</style>

