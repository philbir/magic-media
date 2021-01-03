import Vue from "vue";
import { excuteGraphQL } from "./graphqlClient"
import { addSnack } from "./snackService"

import {
  addItems,
  deleteAlbum,
  getAllAlbums,
  removeFolders,
  searchAlbums,
  setCover,
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
    COVER_SET(state, album) {
      const idx = state.albums.findIndex(x => x.id == album.id);
      if (idx > -1) {
        state.albums[idx].thumbnail = album.thumbnail;
      }
    },
    ALBUM_DELETED(state, id) {
      let idx = state.allAlbums.findIndex(x => x.id == id);
      if (idx > -1) {
        state.allAlbums.splice(idx, 1);
      }
      idx = state.albums.findIndex(x => x.id == id);
      if (idx > -1) {
        state.albums.splice(idx, 1)
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

        addSnack(dispatch, `Media aded to: Album ${result.data.addItemsToAlbum.album.title}`);
      }
    },
    async removeFolders({ dispatch }, input) {
      const result = await excuteGraphQL(() => removeFolders(input), dispatch);

      if (result.success) {
        addSnack(dispatch, `Folder removed from album: ${result.data.removeFoldersFromAlbum.album.title}`);

      }
    },
    async update({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => updateAlbum(input), dispatch);

      if (result.success) {
        commit("ALBUM_UPDATED", result.data.updateAlbum.album);

        addSnack(dispatch, `Album saved: ${result.data.updateAlbum.album.title}`);
      }
    },
    async setCover({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => setCover(input), dispatch);

      if (result.success) {
        commit("COVER_SET", result.data.Album_SetCover.album);

        addSnack(dispatch, "Cover updated");
      }
    },
    async delete({ commit, dispatch }, id) {
      const result = await excuteGraphQL(() => deleteAlbum(id), dispatch);

      if (result.success) {
        commit("ALBUM_DELETED", id);

        addSnack(dispatch, `Album deleted.`);
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
