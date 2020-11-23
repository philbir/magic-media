<template>
  <v-dialog width="800" v-model="isOpen">
    <v-card elevation="2" height="620" class="d-flex flex-column">
      <v-toolbar color="blue" dense dark>
        <v-toolbar-title
          >Edit metadata | {{ mediaIds.length }} items</v-toolbar-title
        >
        <v-spacer></v-spacer>
        <v-btn class="mr-4" color="blue darken-1" @click="setTab('FORM')">
          Metadata
          <v-icon>mdi-format-list-checkbox</v-icon>
        </v-btn>
        <v-btn color="blue darken-3" @click="setTab('MAP')">
          Map
          <v-icon>mdi-map</v-icon>
        </v-btn>
      </v-toolbar>
      <v-card-text>
        <div v-show="selectedTab === 'FORM'">
          <v-form v-model="valid">
            <v-container>
              <v-row dense>
                <v-col md="12">
                  <v-row dense>
                    <v-col md="6">
                      <v-menu
                        ref="menu"
                        v-model="menu"
                        transition="scale-transition"
                        offset-y
                        min-width="290px"
                      >
                        <template v-slot:activator="{ on, attrs }">
                          <v-text-field
                            v-model="form.dateTaken"
                            label="Date taken"
                            v-bind="attrs"
                            v-on="on"
                          ></v-text-field>
                        </template>
                        <v-date-picker
                          ref="picker"
                          v-model="form.dateTaken"
                          :max="new Date().toISOString().substr(0, 10)"
                          min="1920-01-01"
                          @change="saveDate"
                        ></v-date-picker>
                      </v-menu>
                    </v-col>
                  </v-row>
                  <v-row dense>
                    <v-col>
                      <v-text-field
                        v-model="form.address.name"
                        label="Address"
                      ></v-text-field>
                    </v-col>
                  </v-row>
                  <v-row dense> </v-row>
                  <v-row dense>
                    <v-col>
                      <v-text-field
                        v-model="form.address.street"
                        label="Street"
                      ></v-text-field>
                    </v-col>
                    <v-col>
                      <v-text-field
                        v-model="form.address.city"
                        label="City"
                      ></v-text-field>
                    </v-col>
                  </v-row>
                  <v-row dense>
                    <v-col>
                      <v-text-field
                        v-model="form.address.country"
                        label="Country"
                      ></v-text-field>
                    </v-col>
                    <v-col>
                      <v-text-field
                        v-model="form.address.countryCode"
                        label="Country Code"
                      >
                        <template
                          v-slot:prepend
                          v-if="form.address.countryCode"
                        >
                          <img :src="flagSrc" />
                        </template>
                      </v-text-field>
                    </v-col>
                  </v-row>
                  <v-row dense>
                    <v-col>
                      <v-text-field
                        v-model="form.address.district1"
                        label="District1"
                      ></v-text-field>
                    </v-col>
                    <v-col>
                      <v-text-field
                        v-model="form.address.district2"
                        label="District2"
                      ></v-text-field>
                    </v-col>
                  </v-row>
                </v-col>

                <v-col md="7"> </v-col>
              </v-row>

              <v-row style="height: 60px">
                <media-thumbnail-list
                  :ids="mediaIds"
                  @select="selectMedia"
                ></media-thumbnail-list>
              </v-row>
            </v-container>
          </v-form>
        </div>
        <div v-show="selectedTab === 'MAP'">
          <div>
            <gmap-autocomplete
              style="width: 100%; height: 32px"
              placeholder="Search"
              @place_changed="setPlace"
              :select-first-on-enter="true"
            >
            </gmap-autocomplete>
          </div>
          <GmapMap
            :center="map.center"
            :zoom="map.zoom"
            :options="map.options"
            ref="mapRef"
            map-type-id="roadmap"
            @click="clickMap"
            style="width: 100%; height: 490px"
          >
            <GmapMarker v-if="map.marker" :position="map.marker"
          /></GmapMap>
          <div style="height: 30px">{{ form.address.name }} &nbsp;</div>
        </div>
      </v-card-text>
      <v-spacer></v-spacer>

      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>

        <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
        <v-btn color="primary" text @click="save"> Save </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import { gmapApi } from "gmap-vue";
import { parseAddress } from "../../services/googleMapsUtils";
import MediaThumbnailList from "./MediaThumbnailList";
import { getFlagUrl } from "../../services/countryFlags";
import { getMetadata } from "../../services/mediaService";

export default {
  components: { MediaThumbnailList },
  props: {
    show: {
      type: Boolean,
    },
    ids: {
      type: Array,
    },
  },
  data() {
    return {
      menu: false,
      album: {},
      medias: [],
      valid: true,
      selectedTab: "FORM",
      map: {
        center: { lat: 47.28752423541094, lng: 8.533110649108862 },
        zoom: 7,
        options: {
          zoomControl: false,
          mapTypeControl: false,
          scaleControl: false,
          streetViewControl: false,
          rotateControl: false,
          fullscreenControl: false,
          disableDefaultUi: false,
        },
        marker: null,
      },
      form: {
        dateTaken: null,
        address: {
          name: null,
          street: null,
          city: null,
          country: null,
          countryCode: null,
          district1: null,
          district2: null,
        },
      },
    };
  },
  computed: {
    google: function () {
      return gmapApi();
    },
    mediaIds: function () {
      return this.$store.getters["media/selectedMediaIds"];
    },
    flagSrc: function () {
      return getFlagUrl(this.form.address.countryCode, 32);
    },
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
    setPlace: function (place) {
      this.map.center = place.geometry.location;
      this.map.zoom = 10;
      const address = parseAddress(place);
      this.setAddress(address);
      this.map.marker = place.geometry.location;
    },

    clickMap: function (loc) {
      this.map.marker = {
        lat: loc.latLng.lat(),
        lng: loc.latLng.lng(),
      };

      const geocoder = new this.google.maps.Geocoder();
      geocoder.geocode({ location: loc.latLng }, (results, status) => {
        if (status === "OK") {
          if (results.length > 0) {
            const address = parseAddress(results[0]);
            this.setAddress(address);
          }
        } else {
          window.alert("Geocoder failed due to: " + status);
        }
      });
    },
    setAddress: function (address) {
      this.form.address.name = address.name;
      this.form.address.street = `${address.street ?? ""} ${
        address.streetNumber ?? ""
      }`.trim();
      this.form.address.city = address.city;
      this.form.address.country = address.country;
      this.form.address.countryCode = address.countryCode;
      this.form.address.district1 = address.district1;
      this.form.address.district2 = address.district2;
    },
    async selectMedia(id) {
      const result = await getMetadata(id);
      const media = result.data.mediaById;

      if (media.geoLocation != null) {
        const { address, point } = media.geoLocation;
        this.form.address.name = address.name;
        this.form.address.city = address.city;
        this.form.address.street = address.address;
        this.form.address.country = address.country;
        this.form.address.countryCode = address.countryCode;
        this.form.address.district1 = address.distric1;
        this.form.address.district2 = address.distric2;

        const loc = {
          lat: point.coordinates[1],
          lng: point.coordinates[0],
        };
        this.map.center = loc;
        this.map.marker = loc;
        this.map.zoom = 10;
      } else {
        this.form.address.name = null;
        this.form.address.city = null;
        this.form.address.street = null;
        this.form.address.country = null;
        this.form.address.countryCode = null;
        this.form.address.district1 = null;
        this.form.address.district2 = null;
        this.map.marker = null;
      }

      this.form.dateTaken = this.$options.filters.toISODate(media.dateTaken);
    },
    setTab: function (tab) {
      this.selectedTab = tab;
    },
    saveDate() {
      this.dateTakenChanged = true;
    },
    save: function () {
      const input = {
        ids: this.mediaIds,
        dateTaken: this.dateTakenChanged ? this.form.dateTaken : null,
      };

      if (this.map.marker != null) {
        input.geoLocation = {
          latitude: this.map.marker.lat,
          longitude: this.map.marker.lng,
          name: this.form.address.name,
          city: this.form.address.city,
          address: this.form.address.street,
          country: this.form.address.country,
          countryCode: this.form.address.countryCode,
          distric1: this.form.address.district1,
          distric2: this.form.address.district2,
        };
      }

      this.$store.dispatch("media/updateMetadata", input);

      this.close();
    },
    cancel: function () {
      this.close();
    },
    close: function () {
      this.isOpen = false;
      this.medias = [];
      this.map.marker = null;
      this.form.dateTaken = null;
      this.form.address.name = null;
      this.form.address.city = null;
      this.form.address.street = null;
      this.form.address.country = null;
      this.form.address.countryCode = null;
      this.form.address.district1 = null;
      this.form.address.district2 = null;
      this.map.marker = null;
    },
  },
};
</script>

<style scoped>
.map-search {
  top: 0px;
  left: 10px;
  z-index: 1000;
}
</style>

