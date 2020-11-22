<template>
  <v-dialog width="800" v-model="isOpen">
    <v-card elevation="2" min-height="480" class="d-flex flex-column">
      <v-card-title> Update metadata </v-card-title>
      <v-card-text>
        <v-form v-model="valid">
          <v-container>
            <v-row dense>
              <v-col md="5">
                <v-row dense>
                  <v-col>
                    <v-text-field
                      v-model="form.dateTaken"
                      label="Date taken"
                    ></v-text-field>
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
                <v-row dense>
                  <v-col>
                    <v-text-field
                      v-model="form.address.street"
                      label="Street"
                    ></v-text-field>
                  </v-col>
                </v-row>
                <v-row dense>
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
                </v-row>
                <v-row dense>
                  <v-col>
                    <v-text-field
                      v-model="form.address.district1"
                      label="District1"
                    ></v-text-field>
                  </v-col>
                </v-row>
                <v-row dense>
                  <v-col>
                    <v-text-field
                      v-model="form.address.district2"
                      label="District2"
                    ></v-text-field>
                  </v-col>
                </v-row>
              </v-col>

              <v-col md="7">
                <div>
                  <gmap-autocomplete
                    style="width: 100%"
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
                  style="width: 100%; height: 400px"
                >
                  <GmapMarker v-if="map.marker" :position="map.marker"
                /></GmapMap>
              </v-col>
            </v-row>

            <v-row style="height: 60px"> </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-spacer></v-spacer>

      <v-divider></v-divider>

      <v-card-actions class="pa-1">
        <v-spacer></v-spacer>

        <v-btn color="blue darken-1" text @click="cancel"> Cancel </v-btn>
        <v-btn color="primary" text @click="save"> Save </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
/* eslint-disable no-debugger */
import { gmapApi } from "gmap-vue";
import { parseAddress } from "../../services/googleMapsUtils";

export default {
  components: null,
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
          district1: null,
          district2: null,
        },
      },
    };
  },
  watch: {
    ids: function (newValue) {
      console.log(newValue);
    },
  },
  computed: {
    google: function () {
      return gmapApi();
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
            console.log(results);
            console.log(address);
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
      this.form.address.district1 = address.district1;
      this.form.address.district2 = address.district2;
    },
    save: function () {
      this.close();
    },
    cancel: function () {
      this.close();
    },
    close: function () {
      this.isOpen = false;
      this.medias = [];
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

