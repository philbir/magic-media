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
                <v-menu
                  ref="menu"
                  v-model="menu"
                  :close-on-content-click="false"
                  transition="scale-transition"
                  offset-y
                  min-width="290px"
                >
                  <template v-slot:activator="{ on, attrs }">
                    <v-text-field
                      v-model="date"
                      label="Birthday"
                      prepend-icon="mdi-calendar"
                      readonly
                      v-bind="attrs"
                      v-on="on"
                    ></v-text-field>
                  </template>
                  <v-date-picker
                    ref="picker"
                    v-model="date"
                    :max="new Date().toISOString().substr(0, 10)"
                    min="1920-01-01"
                    @change="saveDate"
                  ></v-date-picker>
                </v-menu>
              </v-col>

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
import { DateTime } from "luxon";
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
      menu: false,
      date: null,
      person: {},
      valid: true,
      groupSearch: "",
    };
  },
  watch: {
    menu(val) {
      val && setTimeout(() => (this.$refs.picker.activePicker = "YEAR"));
    },
    personId: function (newValue) {
      if (newValue) {
        const person = this.$store.state.person.persons.find(
          (x) => x.id === newValue
        );
        if (person.dateOfBirth) {
          var date = DateTime.fromISO(person.dateOfBirth);

          if (DateTime.isDateTime(date)) {
            this.date = date.toISODate();
          }
        }
        this.person = {
          ...person,
          groups: person.groups.map((x) => {
            return {
              value: x.id,
              text: x.name,
            };
          }),
        };
      } else {
        this.person = {};
        this.date = null;
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
    groups: function () {
      return this.$store.state.person.groups.map((x) => {
        return {
          text: x.name,
          value: x.id,
        };
      });
    },
  },
  methods: {
    save: function () {
      const groups = [];
      this.person.newGroups = [];

      for (let i = 0; i < this.person.groups.length; i++) {
        const item = this.person.groups[i];
        if (item.value) {
          groups.push(item.value);
        } else {
          this.person.newGroups.push(item);
        }
      }
      this.person.groups = groups;

      this.$store.dispatch("person/update", this.person);
      this.isOpen = false;
    },
    cancel: function () {
      this.isOpen = false;
    },
    saveDate: function (date) {
      this.$refs.menu.save(date);
      this.person.dateOfBirth = date;
    },
  },
};
</script>

<style scoped>
</style>

