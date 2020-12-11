<template>
  <v-dialog width="800" v-model="isOpen">
    <v-card
      elevation="2"
      min-height="560"
      class="d-flex flex-column"
      v-if="album"
    >
      <v-progress-linear
        v-if="album.id == undefined"
        indeterminate
      ></v-progress-linear>
      <v-card-title>
        {{ album.title }}
        <div class="country-flag-container">
          <img
            v-for="(country, i) in album.countries"
            :key="i"
            :src="flagUrl(country)"
          />
        </div>
      </v-card-title>
      <v-card-text>
        <div v-show="view === 'details'">
          <v-form v-model="valid">
            <v-container v-if="album.id">
              <v-row>
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="album.title"
                    label="Title"
                    required
                  ></v-text-field>

                  <v-row v-for="(item, i) in dataItems" :key="i">
                    <v-col cols="12" sm="4">{{ item.label }}</v-col>
                    <v-col cols="12" sm="4" class="font-weight-bold">{{
                      item.value
                    }}</v-col>
                  </v-row>
                </v-col>

                <v-col cols="12" md="6">
                  <v-expansion-panels accordion :value="0">
                    <v-expansion-panel v-if="album && album.persons.length">
                      <v-expansion-panel-header>
                        Persons
                      </v-expansion-panel-header>
                      <v-expansion-panel-content>
                        <v-list height="200" style="overflow: auto">
                          <v-list-item
                            v-for="person in album.persons"
                            :key="person.personId"
                          >
                            <v-list-item-avatar size="32">
                              <img
                                :alt="person.name"
                                :src="`/api/media/thumbnail/face/${person.faceId}`"
                              />
                            </v-list-item-avatar>
                            <v-list-item-content>
                              <v-list-item-title>
                                <strong> {{ person.name }}</strong> ({{
                                  person.count
                                }})</v-list-item-title
                              >
                            </v-list-item-content>
                          </v-list-item>
                        </v-list>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                    <v-expansion-panel v-if="album && album.countries.length">
                      <v-expansion-panel-header>
                        Countries
                      </v-expansion-panel-header>
                      <v-expansion-panel-content>
                        <v-list height="200" style="overflow: auto">
                          <v-list-item
                            v-for="country in album.countries"
                            :key="country.code"
                          >
                            <v-list-item-avatar size="32">
                              <img
                                :alt="country.name"
                                :src="flagUrl(country)"
                              />
                            </v-list-item-avatar>
                            <v-list-item-content>
                              <v-list-item-title :title="cityString(country)">
                                <strong> {{ country.name }}</strong> ({{
                                  country.count
                                }})</v-list-item-title
                              >
                              <v-list-item-subtitle>
                                <span>{{ cityString(country) }}</span>
                              </v-list-item-subtitle>
                            </v-list-item-content>
                          </v-list-item>
                        </v-list>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                  </v-expansion-panels>
                </v-col>
              </v-row>

              <v-row style="height: 60px">
                <album-media :items="medias"></album-media
              ></v-row>
            </v-container>
          </v-form>
        </div>
        <div v-show="view === 'settings'">
          <v-row>
            <v-col sm="6">
              <div v-if="album.folders">
                <h4>Folders</h4>
                <v-chip
                  v-for="(folder, i) in album.folders"
                  :key="i"
                  class="ma-2"
                  text-color="white"
                  color="blue darken-4"
                  close
                  @click:close="removeFolder(folder)"
                >
                  {{ folder }}
                </v-chip>
              </div>
              <div v-if="album.filters">
                <h4>Filters</h4>
                <v-chip
                  v-for="filter in album.filters"
                  :key="filter.key"
                  class="ma-2"
                  close
                  text-color="white"
                  color="blue darken-4"
                  @click:close="removeFilter(filter.key)"
                >
                  {{ filter.name }} : {{ filter.description }}
                </v-chip>
              </div>
            </v-col>
            <v-col sm="6"><h4>Shared with</h4></v-col>
          </v-row>
        </div>
      </v-card-text>
      <v-spacer></v-spacer>

      <v-divider></v-divider>

      <v-card-actions class="pa-1">
        <v-btn color="blue darken-1" text @click="toggleView">
          {{ view === "settings" ? "Details" : "Settings" }}
        </v-btn>

        <v-spacer></v-spacer>

        <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
        <v-btn color="red darken-1" text @click="deleteAlbum"> Delete </v-btn>
        <v-btn
          color="primary"
          text
          @click="save"
          :disabled="album.title == originalTitle"
        >
          Save
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import { getFlagUrl } from "../../services/countryFlags";
import { getAlbumById, getAlbumMedia } from "../../services/albumService";
import AlbumMedia from "./AlbumMedia.vue";
import { mapActions } from "vuex";
export default {
  components: { AlbumMedia },
  props: {
    show: {
      type: Boolean,
    },
    albumId: {
      type: String,
    },
  },
  data() {
    return {
      menu: false,
      album: {},
      medias: [],
      valid: true,
      originalTitle: null,
      view: "details",
    };
  },
  watch: {
    albumId: function (newValue) {
      if (newValue) {
        this.loadAlbum(newValue);
      } else {
        this.album = {};
        this.medias = [];
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
    dataItems: function () {
      if (this.album) {
        return [
          {
            label: "Start date",
            value: this.$options.filters.dateformat(this.album.startDate),
          },
          {
            label: "End date",
            value: this.$options.filters.dateformat(this.album.endDate),
          },
          {
            label: "Image count",
            value: this.album.imageCount,
          },
          {
            label: "Video count",
            value: this.album.videoCount,
          },
        ];
      }
      return [];
    },
  },
  methods: {
    ...mapActions("album", {
      removeFolders: "removeFolders",
      saveAlbum: "update",
    }),
    async loadAlbum(id) {
      const res = await getAlbumById(id);
      this.album = res.data.album;
      this.originalTitle = this.album.title;

      const mediaRes = await getAlbumMedia(id);

      this.medias = mediaRes.data.searchMedia.items;
    },
    save: function () {
      this.saveAlbum({
        title: this.album.title,
        id: this.album.id,
      });
      this.close();
    },
    toggleView: function () {
      this.view = this.view === "settings" ? "details" : "settings";
    },
    cancel: function () {
      this.close();
    },
    deleteAlbum: function () {
      this.close();
    },
    removeFolder: function (folder) {
      const idx = this.album.folders.findIndex((x) => x === folder);
      this.album.folders.splice(idx, 1);
      this.removeFolders({
        albumId: this.album.id,
        folders: [folder],
      });
    },
    removeFilter: function (key) {
      const idx = this.album.filters.findIndex((x) => x.key === key);
      this.album.filters.splice(idx, 1);
    },
    flagUrl: function (country) {
      return getFlagUrl(country.code, 32);
    },
    cityString: function (country) {
      return country.cities.map((c) => c.name).join(" | ");
    },
    close: function () {
      this.isOpen = false;
      this.album = {};
      this.medias = [];
      this.view = "details";
    },
  },
};
</script>

<style lang="scss" scoped>
.country-flag-container {
  position: absolute;
  right: 20px;
  img {
    width: 28px;
    height: 28px;
    padding-left: 4px;
  }
}
</style>


