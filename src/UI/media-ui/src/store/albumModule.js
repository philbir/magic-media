import Vue from "vue";
import { excuteGraphQL } from "./graphqlClient"

import {
  addItems,
  getAllAlbums,
  removeFolders,
  searchAlbums,
  updateAlbum
} from "../services/albumService";

const albumModule = {
  namespaced: true,
  state: () => ({
    albums: [],
    allAlbums: [],
    filter: {
      searchText: "",
      pageSize: 25,
      pageNr: 0
    }
  }),
  mutations: {
    ITEM_ADDED(state, album) {
      const albums = state.allAlbums.filter(x => x.id == album.id);
      if (albums.length === 0) state.allAlbums.push(album);
    },
    ALBUM_UPDATED(state, album) {
      let idx = state.allAlbums.findIndex(x => x.id == album.id);
      if (idx > -1) {
        state.allAlbums[idx].title = album.title;
      }
      idx = state.albums.findIndex(x => x.id == album.id);
      if (idx > -1) {
        state.albums[idx].title = album.title;
      }
    },
    ALL_ALBUMS_LOADED(state, albums) {
      state.allAlbums = [...albums];
    },
    SEARCH_ITEMS_LOADED(state, result) {
      Vue.set(state, "albums", [...result.items]);
    },
    FILTER_SET(state, filter) {
      state.filter = { ...this.state.filter, ...filter };
    }
  },
  actions: {
    async addItems({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => addItems(input), dispatch);

      if (result.success) {
        commit("ITEM_ADDED", result.data.addItemsToAlbum.album);

        dispatch(
          "snackbar/addSnack",
          {
            text: `Media aded to: Album ${result.data.addItemsToAlbum.album.title}`,
            type: "SUCCESS"
          },
          { root: true }
        );
      }
    },
    async removeFolders({ dispatch }, input) {
      const result = await excuteGraphQL(() => removeFolders(input), dispatch);

      if (result.success) {
        dispatch(
          "snackbar/addSnack",
          {
            text: `Folder removed from album: ${result.data.removeFoldersFromAlbum.album.title}`,
            type: "SUCCESS"
          },
          { root: true }
        );
      }
    },
    async update({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => updateAlbum(input), dispatch);

      if (result.success) {
        commit("ALBUM_UPDATED", result.data.updateAlbum.album);

        dispatch(
          "snackbar/addSnack",
          {
            text: `Album saved: ${result.data.updateAlbum.album.title}`,
            type: "SUCCESS"
          },
          { root: true }
        );
      }
    },
    async getAll({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getAllAlbums(), dispatch);

      if (result.success) {
        commit("ALL_ALBUMS_LOADED", result.data.allAlbums);

      }
    },
    async search({ state, commit, dispatch }) {
      const result = await excuteGraphQL(() => searchAlbums(state.filter), dispatch);

      if (result.success) {
        commit("SEARCH_ITEMS_LOADED", result.data.searchAlbums);
      }
    },
    filter: function ({ commit, dispatch }, filter) {
      commit("FILTER_SET", filter);
      dispatch("search");
    }
  },
  getters: {}
};

export default albumModule;
