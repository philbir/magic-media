<template>
  <div>
    <v-dialog width="800" v-model="isOpen">
      <v-card :loading="loading">
        <v-toolbar flat color="blue darken-4" dark>
          <v-toolbar-title>{{ user.name }}</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-btn icon>
            <v-icon color="white" @click="cancel"
              >mdi-arrow-left-circle
            </v-icon>
          </v-btn>
        </v-toolbar>

        <v-tabs vertical v-model="tab" v-if="user">
          <v-tab>
            <v-icon left> mdi-account </v-icon>
          </v-tab>
          <v-tab>
            <v-icon left> mdi-image-album </v-icon>
          </v-tab>
          <v-tab>
            <v-icon left> mdi-ticket</v-icon>
          </v-tab>
          <v-tab>
            <v-icon left> mdi-chart-line </v-icon>
          </v-tab>

          <v-tab-item>
            <v-card flat :height="cardHeight" v-if="user">
              <v-card-text>
                <v-form v-model="valid">
                  <v-container>
                    <v-row>
                      <v-col md="6">
                        <v-text-field
                          v-model="user.name"
                          label="Name"
                          required
                        ></v-text-field>
                      </v-col>

                      <v-col md="6">
                        <v-text-field
                          v-model="user.email"
                          label="Email"
                          required
                        ></v-text-field>
                      </v-col>
                      <v-col cols="12" md="12">
                        <v-autocomplete
                          v-model="user.roles"
                          :items="roles"
                          text-color="white"
                          label="Roles"
                          chips
                          multiple
                          deletable-chips
                        >
                        </v-autocomplete>
                      </v-col>
                    </v-row>
                  </v-container>
                </v-form>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card :height="cardHeight" flat>
              <v-card-text>
                Albums shared with {{ user.name }}

                <v-autocomplete
                  v-model="user.sharedAlbums"
                  :items="allAlbums"
                  text-color="white"
                  chips
                  item-text="display"
                  item-value="id"
                  multiple
                  return-object
                  @change="albumChanged"
                >
                  <template v-slot:selection="data">
                    <v-chip
                      v-bind="data.attrs"
                      :input-value="data.selected"
                      text-color="white"
                      color="blue darken-4"
                      close
                      @click="data.select"
                      @click:close="removeAlbum(data.item)"
                    >
                      {{ data.item.display }}
                    </v-chip>
                  </template>
                </v-autocomplete>
                <br />
                * {{ user.name }} is in album
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card :height="cardHeight" flat>
              <v-card-text>
                <div v-if="user.state === 'NEW'">Create Invite</div>
                <div v-if="user.state === 'INVITED'">
                  Code:
                  <h3>{{ user.invitationCode }}</h3>

                  <br />
                  <a
                    :href="`https://id.birbaum.me/invite/${user.invitationCode}`"
                    v-text="
                      `https://id.birbaum.me/invite/${user.invitationCode}`
                    "
                  ></a>
                </div>
                <div v-if="user.state === 'ACTIVE'">
                  User allready activated
                </div>
              </v-card-text>
            </v-card>
          </v-tab-item>
          <v-tab-item>
            <v-card :height="cardHeight" flat>
              <v-card-text>
                <h4>Analytics</h4>
                <br />
                <p>
                  Total authorized on media:
                  <strong>{{ user.authorizedOnMediaCount }}</strong>
                </p>
                <p>
                  Total albums:
                  <strong>{{
                    user.sharedAlbums ? user.sharedAlbums.length : 0
                  }}</strong>
                </p>
                <p>
                  In albums:
                  <strong>{{ user.inAlbum ? user.inAlbum.length : 0 }}</strong>
                </p>
              </v-card-text>
            </v-card>
          </v-tab-item>
        </v-tabs>
        <v-card-actions class="pa-1">
          <v-spacer></v-spacer>
          <v-btn color="primary" v-if="tab == 0" text @click="save">
            Save
          </v-btn>
          <v-btn color="primary" v-if="tab == 1" text @click="saveAlbums">
            Save albums
          </v-btn>
          <v-btn
            color="primary"
            v-if="tab == 2 && user.state === 'NEW'"
            text
            @click="createInvite"
          >
            Create invite
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    >
  </div>
</template>
<script>
import { getById } from "../../services/userService";

const albumCompare = (a, b) => {
  if (a.inAlbum < b.inAlbum) {
    return 1;
  }
  if (a.inAlbum > b.inAlbum) {
    return -1;
  }
  return 0;
};

export default {
  components: {},
  props: {
    show: {
      type: Boolean,
    },
    userId: {
      type: String,
    },
  },
  data() {
    return {
      date: null,
      user: {},
      valid: true,
      tab: null,
      loading: false,
      cardHeight: 340,
    };
  },
  watch: {
    userId: function (newValue) {
      if (newValue) {
        this.load(newValue);
      } else {
        this.user = {};
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
    roles: function () {
      return ["Admin", "Demo"];
    },
    allAlbums: function () {
      return this.$store.state.album.allAlbums
        .map((x) => {
          let index = -1;
          if (this.user.inAlbum) {
            index = this.user.inAlbum.findIndex((a) => a.id === x.id);
          }

          if (index > -1) {
            x.inAlbum = true;
            x.display = x.title + " *";
          } else {
            x.inAlbum = false;
            x.display = x.title;
          }
          return x;
        })
        .sort(albumCompare);
    },
  },
  methods: {
    async load(userId) {
      this.loading = true;
      const result = await getById(userId);
      this.loading = false;
      this.user = result.data.user;
      this.tabs = 0;
    },
    save: function () {
      this.$store.dispatch("user/update", this.user);
      this.isOpen = false;
    },
    saveAlbums: function () {
      this.$store.dispatch("user/saveSharedAlbums", {
        userId: this.user.id,
        albums: this.user.sharedAlbums.map((x) => x.id),
      });
    },
    cancel: function () {
      this.isOpen = false;
    },
    removeRole: function (role) {
      console.log(role);
    },
    createInvite: function () {
      this.$store.dispatch("user/createInvite", this.user.id);
      const self = this;
      //TODO: Replace with signalR
      window.setTimeout(() => {
        self.load(self.user.id);
      }, 3000);
    },
    albumChanged: function (selected) {
      console.log(selected);
    },
    removeAlbum: function (album) {
      const idx = this.user.sharedAlbums.findIndex((x) => x.id == album.id);
      this.user.sharedAlbums.splice(idx, 1);
    },
  },
};
</script>

<style scoped>
</style>

