import { addItems, getAllAlbums, searchAlbums } from "../services/albumService";
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
    ALL_ALBUMS_LOADED(state, albums) {
      state.allAlbums = [...albums];

    },
    SEARCH_ITEMS_LOADED(state, result) {
      state.albums = [...result.items];

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
    }
  },
  getters: {}
};

export default albumModule;
