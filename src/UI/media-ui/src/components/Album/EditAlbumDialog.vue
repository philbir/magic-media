<template>
  <v-dialog width="800" v-model="isOpen">
    <v-card
      elevation="2"
      min-height="480"
      class="d-flex flex-column"
      v-if="album"
    >
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
                      <v-list>
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
                      <v-list>
                        <v-list-item
                          v-for="country in album.countries"
                          :key="country.code"
                        >
                          <v-list-item-avatar size="32">
                            <img :alt="country.name" :src="flagUrl(country)" />
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
      </v-card-text>
      <v-spacer></v-spacer>

      <v-divider></v-divider>

      <v-card-actions class="pa-1">
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
import { getAlbumMedia } from "../../services/albumService";
import AlbumMedia from "./AlbumMedia.vue";
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
    };
  },
  watch: {
    albumId: function (newValue) {
      if (newValue) {
        const album = this.$store.state.album.albums.find(
          (x) => x.id === newValue
        );
        this.album = { ...album };

        this.originalTitle = this.album.title;
        getAlbumMedia(this.album.id).then((res) => {
          this.medias = res.data.searchMedia.items;
        });
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
    save: function () {
      this.$store.dispatch("album/update", {
        title: this.album.title,
        id: this.album.id,
      });
      this.close();
    },
    cancel: function () {
      this.close();
    },
    deleteAlbum: function () {
      this.close();
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
    },
  },
};
</script>

<style scoped>
</style>

