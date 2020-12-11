<template>
  <div>
    <v-container>
      <v-row style="overflow-x: auto; width: 100%; height: 90vh">
        <v-col v-for="album in albums" :key="album.id" sm="3" lg="4">
          <v-card width="400">
            <v-img height="150" :src="imageUrl(album)"></v-img>

            <v-card-title class="font-weight-bold">
              <div class="country-flag-container">
                <img
                  v-for="(country, i) in album.countries.slice(0, 4)"
                  :key="i"
                  :src="flagUrl(country)"
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
        editAlbumId = null;
      "
    />
  </div>
</template>

<script>
import EditAlbumDialog from "./EditAlbumDialog";
import { getFlagUrl } from "../../services/countryFlags";

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
      return this.$store.state.album.albums;
    },
  },
  methods: {
    imageUrl: function (album) {
      if (album.thumbnail) {
        return album.thumbnail.dataUrl;
      } else {
        return null;
      }
    },
    editClick: function (id) {
      this.editAlbumId = id;
      this.showEditDialog = true;
    },
    flagUrl: function (country) {
      return getFlagUrl(country.code);
    },
  },
};
</script>

<style lang="scss" scoped>
.country-flag-container {
  position: absolute;
  right: 20px;

  img {
    margin-left: 4px;
    margin-top: 10px;
    width: 24px;
    height: 24px;
  }
}
</style>
