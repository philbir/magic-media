import { addItems, getAllAlbums, searchAlbums, updateAlbum } from "../services/albumService";
import Vue from "vue";
/* eslint-disable no-debugger */

const albumModule = {
  namespaced: true,
  state: () => ({
    albums: [],
    allAlbums: [],
    filter: {
      searchText: "",
      pageSize: 25,
      pageNr: 0
    },
  }),
  mutations: {
    ITEM_ADDED(state, album) {
      const albums = state.allAlbums.filter(x => x.id == album.id);
      if (albums.length === 0) state.allAlbums.push(album);
    },
    ALBUM_UPDATED(state, album) {
      debugger;
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
      console.log(result)

      Vue.set(state, "albums", [...result.items]);
    },
    FILTER_SET(state, filter) {
      state.filter = { ... this.state.filter, ...filter }
    }
  },
  actions: {
    async addItems({ commit, dispatch }, input) {
      try {
        const res = await addItems(input);
        commit("ITEM_ADDED", res.data.addItemsToAlbum.album);

        dispatch('snackbar/addSnack', {
          text: `Media aded to: Album ${res.data.addItemsToAlbum.album.title}`,
          type: 'SUCCESS'
        }, { root: true });

      } catch (ex) {
        console.error(ex);
      }
    },
    async update({ commit, dispatch }, input) {
      try {
        const res = await updateAlbum(input);

        commit("ALBUM_UPDATED", res.data.updateAlbum.album);

        dispatch('snackbar/addSnack', {
          text: `Album saved: ${res.data.updateAlbum.album.title}`,
          type: 'SUCCESS'
        }, { root: true });

      } catch (ex) {
        console.error(ex);
      }
    },
    async getAll({ commit }) {
      try {
        const res = await getAllAlbums();
        commit("ALL_ALBUMS_LOADED", res.data.allAlbums);
      } catch (ex) {
        console.error(ex);
      }
    },
    async search({ state, commit }) {
      try {
        const res = await searchAlbums(state.filter);
        commit("SEARCH_ITEMS_LOADED", res.data.searchAlbums);
      } catch (ex) {
        console.error(ex);
      }
    },
    filter: function ({ commit, dispatch }, filter) {
      commit('FILTER_SET', filter)
      dispatch("search");
    }
  },
  getters: {}
};

export default albumModule;
