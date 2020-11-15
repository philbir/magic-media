<template>
  <div>
    <v-container>
      <v-row style="overflow-x: auto; height: 600px">
        <v-col v-for="album in albums" :key="album.id" sm="3" lg="4">
          <v-card width="400">
            <v-img height="150" :src="album.thumbnail.dataUrl"></v-img>

            <v-card-title class="font-weight-bold">
              <div class="country-flag-container">
                <img
                  v-for="(country, i) in album.countries"
                  :key="i"
                  :src="`https://www.countryflags.io/${country.code}/shiny/24.png`"
                />
              </div>
              <h4>{{ album.title }}</h4>
            </v-card-title>
            <v-card-subtitle>
              {{ album.startDate | dateformat("DATE_SHORT") }} -
              {{ album.endDate | dateformat("DATE_SHORT") }}
            </v-card-subtitle>
            <v-card-text> </v-card-text>

            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn outlined rounded text @click="editClick(album.id)">
                Edit
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
    <EditAlbumDialog
      :albumId="editAlbumId"
      :show="showEditDialog"
      @close="
        showEditDialog = false;
        editalbumId = null;
      "
    />
  </div>
</template>

<script>
/* eslint-disable no-debugger */
import EditAlbumDialog from "./EditAlbumDialog";

export default {
  components: { EditAlbumDialog },
  created() {
    this.$store.dispatch("album/search");
  },
  data() {
    return {
      showEditDialog: false,
      editAlbumId: null,
    };
  },
  computed: {
    filters: function () {
      return this.$store.state.album.filter;
    },
    albums: function () {
      return this.$store.state.album.albums.filter((x) => {
        if (this.filters.searchText === "") {
          return true;
        } else {
          return x.name
            .toLowerCase()
            .includes(this.filters.searchText.toLowerCase());
        }
      });
    },
  },
  methods: {
    editClick: function (id) {
      this.editAlbumId = id;
      this.showEditDialog = true;
    },
  },
};
</script>

<style lang="scss">
.country-flag-container {
  position: absolute;
  right: 20px;

  img {
    margin-left: 2px;
    margin-top: 10px;
  }
}
</style>
