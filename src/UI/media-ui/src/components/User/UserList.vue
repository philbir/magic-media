<template>
  <div>
    <v-container style="overflow-x: auto; height: 92vh; width: 100%">
      <v-row>
        <v-col v-for="user in users" :key="user.id" sm="3" lg="4">
          <v-card width="320" height="220">
            <v-list-item two-line>
              <v-list-item-avatar>
                <v-img :src="thumbnail(user)"></v-img>
              </v-list-item-avatar>

              <v-list-item-content>
                <v-list-item-title v-html="user.name"></v-list-item-title>
                <v-list-item-subtitle
                  v-html="user.email"
                ></v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>

            <v-card-text>
              <v-row>
                <v-col
                  >State: <strong>{{ user.state }} </strong></v-col
                >
                <v-col>
                  <v-chip-group multiple>
                    <v-chip
                      v-for="role in user.roles"
                      small
                      :key="role"
                      v-text="role"
                      color="blue lighten-4"
                    >
                    </v-chip>
                  </v-chip-group>
                </v-col>
              </v-row>
            </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>

              <v-btn
                outlined
                rounded
                text
                @click="editClick(user.id)"
                v-if="userActions.user.edit"
              >
                Edit
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
    <edit-user-dialog
      :userId="editUserId"
      :show="editUserId != null"
      @close="editUserId = null"
    ></edit-user-dialog>
  </div>
</template>

<script>
import { mapGetters, mapState } from "vuex";
import EditUserDialog from "./EditUserDialog.vue";
export default {
  components: { EditUserDialog },
  created() {
    this.search();
  },
  data() {
    return {
      editUserId: null,
    };
  },
  computed: {
    ...mapState("user", { users: "list" }),
    ...mapGetters("user", ["userActions"]),
  },
  methods: {
    thumbnail: function (user) {
      if (user.person && user.person.thumbnail) {
        return user.person.thumbnail.dataUrl;
      } else {
        return null;
      }
    },
    search: function () {
      this.$store.dispatch("user/search");
    },
    editClick: function (id) {
      this.editUserId = id;
    },
  },
};
</script>

<style>
</style>