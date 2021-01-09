<template>
  <div>
    <v-dialog width="800" v-model="isOpen">
      <v-card :loading="loading">
        <v-toolbar flat color="blue darken-4" dark>
          <v-toolbar-title>{{ person.name }}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn icon>
            <v-icon color="white" @click="cancel"
              >mdi-arrow-left-circle
            </v-icon>
          </v-btn>
        </v-toolbar>

        <v-tabs vertical v-model="tab" v-if="person">
          <v-tab>
            <v-icon left> mdi-account </v-icon>
          </v-tab>
          <v-tab>
            <v-icon left> mdi-web </v-icon>
          </v-tab>
          <v-tab v-if="userActions.person.delete">
            <v-icon left> mdi-bomb</v-icon>
          </v-tab>

          <v-tab-item>
            <v-card flat :height="cardHeight">
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
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card :height="cardHeight" flat>
              <v-card-text> </v-card-text>
              <div v-if="person.user">
                {{ person.name }} has allready a user.

                <br />
                State: <strong>{{ person.user.state }}</strong>
              </div>
              <div v-if="person.user == null">
                Create user
                <v-form v-model="valid">
                  <v-container>
                    <v-row>
                      <v-col md="12">
                        <v-text-field
                          v-model="createUserForm.email"
                          label="Email"
                          required
                        ></v-text-field>
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col md="12">
                        <v-text-field
                          v-model="createUserForm.mobile"
                          label="Mobile phone"
                          required
                        ></v-text-field>
                      </v-col>
                    </v-row>
                  </v-container>
                </v-form>
              </div>
            </v-card>
          </v-tab-item>
          <v-tab-item v-if="userActions.person.delete">
            <v-card :height="cardHeight" flat>
              <v-card-text>
                Danger zone
                <br />
                <v-btn class="mt-4" color="error" @click="deletePerson">
                  <v-icon left> mdi-delete-outline </v-icon>Delete person</v-btn
                >
              </v-card-text>
            </v-card>
          </v-tab-item>
        </v-tabs>
        <v-card-actions class="pa-1">
          <v-spacer></v-spacer>
          <v-btn
            color="primary"
            v-if="tab == 0 && userActions.person.edit"
            text
            @click="save"
          >
            Save
          </v-btn>
          <v-btn
            color="primary"
            v-if="tab == 1 && person.user == null"
            text
            @click="createUser"
          >
            Create user
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>
<script>
import { DateTime } from "luxon";
import { mapActions, mapGetters } from "vuex";
import { getPersonById } from "../../services/personService";

export default {
  components: {},
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
      tab: 0,
      cardHeight: 340,
      loading: false,
      menu: false,
      date: null,
      person: {},
      valid: true,
      groupSearch: "",
      createUserForm: {
        email: null,
        mobile: null,
      },
    };
  },
  watch: {
    menu(val) {
      val && setTimeout(() => (this.$refs.picker.activePicker = "YEAR"));
    },
    personId: function (newValue) {
      if (newValue) {
        this.load(newValue);
      } else {
        this.person = {};
        this.date = null;
      }
    },
  },
  computed: {
    ...mapGetters("user", ["userActions"]),
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
    ...mapActions("user", ["createFromPerson"]),
    async load(id) {
      this.loading = true;

      const result = await getPersonById(id);
      this.person = result.data.person;

      if (this.person.dateOfBirth) {
        var date = DateTime.fromISO(this.person.dateOfBirth);

        if (DateTime.isDateTime(date)) {
          this.date = date.toISODate();
        }
      }
      this.person = {
        ...this.person,
        groups: this.person.groups.map((x) => {
          return {
            value: x.id,
            text: x.name,
          };
        }),
      };

      this.loading = false;
    },
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
    deletePerson: function () {
      this.$store.dispatch("person/delete", this.person.id);
      this.$store.dispatch("person/search", this.person.id);
      this.cancel();
    },
    createUser: function () {
      this.createFromPerson({
        personId: this.person.id,
        email: this.createUserForm.email,
      }).then(() => {
        this.load(this.person.id);
      });
    },
  },
};
</script>

<style scoped>
</style>

