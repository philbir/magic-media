import Vue from "vue";

import {
  getById,
  getFolderTree,
  getSearchFacets,
  moveMedia,
  recycleMedia,
  searchMedia,
  toggleFavorite
} from "../services/mediaService";

/* eslint-disable no-debugger */

const getMediaIdsFromIndexes = state => {
  const ids = [];
  for (let i = 0; i < state.selectedIndexes.length; i++) {
    ids.push(state.list[state.selectedIndexes[i]].id);
  }
  return ids;
};

const mediaModule = {
  namespaced: true,
  state: () => ({
    uploadDialog: {
      open: false
    },
    list: [],
    facets: null,
    folderTree: {
      name: "Home",
      children: []
    },
    current: null,
    totalLoaded: 0,
    currentMediaId: null,
    listLoading: false,
    selectedIndexes: [],
    hasMore: true,
    isEditMode: false,
    thumbnailSize: "M",
    filter: {
      pageNr: 0,
      pageSize: 200,
      countries: [],
      cities: [],
      persons: [],
      albumId: null,
      geoRadius: null
    }
  }),
  mutations: {
    MEDIAITEMS_LOADED(state, result) {
      const max = 600;
      const current = [...state.list];
      if (current.length > max) {
        current.splice(0, state.filter.pageSize);
      }

      Vue.set(state, "list", [...current, ...result.items]);
      state.listLoading = false;
      state.totalLoaded = state.totalLoaded + result.items.length;
      state.hasMore = result.hasMore;
    },
    DETAILS_LOADED(state, media) {
      state.currentMediaId = media.id;
      state.current = Object.assign({}, media);
    },
    FOLDER_TREE_LOADED(state, tree) {
      state.folderTree = Object.assign({}, tree);
    },
    FILTER_THUMBNAIL_SIZE_SET(state, size) {
      state.list = [];
      state.filter.pageNr = 0;
      state.thumbnailSize = size;
      state.selectedIndexes = [];
    },
    FILTER_PERSONS_SET(state, persons) {
      state.filter.persons = persons;
    },
    FILTER_COUNTRY_SET(state, countries) {
      state.filter.countries = countries;
    },
    FILTER_CITY_SET(state, cities) {
      state.filter.cities = cities;
    },
    FILTER_FOLDER_SET(state, folder) {
      state.filter.folder = folder;
    },
    FILTER_ALBUM_SET(state, albumId) {
      state.filter.albumId = albumId;
    },
    FILTER_GEO_SET(state, geo) {
      state.filter.geoRadius = geo;
    },
    PAGE_NR_INC(state) {
      state.filter.pageNr++;
    },
    UPLOAD_DIALOG_TOGGLED: function (state, open) {
      state.uploadDialog.open = open;
    },
    SET_MEDIALIST_LOADING: function (state, isloading) {
      state.listLoading = isloading;
    },
    SEARCH_FACETS_LOADED: function (state, facets) {
      Vue.set(state, "facets", facets);
    },
    RESET_FILTER: function (state) {
      state.list = [];
      state.filter.pageNr = 0;
      state.totalLoaded = 0;
      state.selectedIndexes = [];
    },
    MEDIA_CLOSED: function (state) {
      state.currentMediaId = null;
      state.current = null;
    },
    EDIT_MODE_TOGGLE: function (state, value) {
      state.isEditMode = value;
      if (!value) {
        state.selectedIndexes = [];
      }
    },
    SELECTED: function (state, idx) {
      const current = [...state.selectedIndexes];
      const i = current.indexOf(idx);
      if (i > -1) {
        current.splice(i, 1);
      } else {
        current.push(idx);
      }

      Vue.set(state, "selectedIndexes", current);
    },
    ALL_SELECTED: function (state) {
      state.selectedIndexes = [...Array(state.list.length).keys()];
    },
    CLEAR_SELECTED: function (state) {
      state.selectedIndexes = [];
    },
    OPERATION_COMMITED: function (state) {
      var mediaIds = getMediaIdsFromIndexes(state);

      const current = [...state.list];
      for (let i = 0; i < mediaIds.length; i++) {
        var idx = current.findIndex(x => x.id === mediaIds[i]);
        current.splice(idx, 1);
      }
      state.selectedIndexes = [];
      Vue.set(state, "list", current);
    },
    FAVORITE_TOGGLED: function (state) {
      state.current.isFavorite = !state.current.isFavorite;
      var idx = state.list.findIndex(x => x.id === state.current.id);
      if (idx > -1) {
        state.list[idx].isFavorite = state.current.isFavorite;
      }
    }
  },
  actions: {
    async search({ commit, state }) {
      try {
        commit("SET_MEDIALIST_LOADING", true);

        const res = await searchMedia(state.filter, state.thumbnailSize);
        commit("MEDIAITEMS_LOADED", res.data.searchMedia);
      } catch (ex) {
        this.$magic.snack("Error loading", "ERROR");
      }
    },
    async loadMore({ commit, state, dispatch }) {
      if (state.hasMore) {
        commit("PAGE_NR_INC");
        dispatch("search");
      }
    },
    async show({ commit }, id) {
      try {
        const res = await getById(id);
        commit("DETAILS_LOADED", res.data.mediaById);
      } catch (ex) {
        console.error(ex);
      }
    },
    async getFolderTree({ commit }) {
      try {
        const res = await getFolderTree();
        commit("FOLDER_TREE_LOADED", res.data.folderTree);
      } catch (ex) {
        this.$magic.snack("Error loading", "ERROR");
      }
    },
    async moveSelected({ commit, state, dispatch }, newLocation) {
      try {
        const ids = getMediaIdsFromIndexes(state);
        const res = await moveMedia({
          ids,
          newLocation
        });

        commit("OPERATION_COMMITED", res.data.moveMedia.operationId);

        dispatch(
          "snackbar/operationStarted",
          {
            id: res.data.moveMedia.operationId,
            type: "INFO",
            title: "Move media",
            totalCount: ids.length,
            text: "Move media"
          },
          { root: true }
        );
      } catch (ex) {
        console.error(ex);
        this.$magic.snack("Error loading", "ERROR");
      }
    },
    async recycleSelected({ commit, state, dispatch }) {
      try {
        const ids = getMediaIdsFromIndexes(state);
        const res = await recycleMedia({
          ids,
        });

        commit("OPERATION_COMMITED", res.data.recycleMedia.operationId);

        dispatch(
          "snackbar/operationStarted",
          {
            id: res.data.moveMedia.operationId,
            type: "INFO",
            title: "Recycle media",
            totalCount: ids.length,
            text: "Recycle media"
          },
          { root: true }
        );
      } catch (ex) {
        console.error(ex);
        this.$magic.snack("Error loading", "ERROR");
      }
    },
    close({ commit }) {
      commit("MEDIA_CLOSED");
    },
    setThumbnailSize({ dispatch, commit }, size) {
      commit("FILTER_THUMBNAIL_SIZE_SET", size);
      dispatch("search");
    },
    setPersonFilter({ dispatch, commit }, persons) {
      commit("RESET_FILTER");
      commit("FILTER_PERSONS_SET", persons);
      dispatch("search");
    },
    setCountryFilter({ dispatch, commit }, countries) {
      commit("RESET_FILTER");
      commit("FILTER_COUNTRY_SET", countries);
      dispatch("search");
    },
    setCityFilter({ dispatch, commit }, cities) {
      commit("RESET_FILTER");
      commit("FILTER_CITY_SET", cities);
      dispatch("search");
    },
    setFolderFilter({ dispatch, commit }, folder) {
      commit("RESET_FILTER");
      commit("FILTER_FOLDER_SET", folder);
      dispatch("search");
    },
    setAlbumFilter({ dispatch, commit }, albumId) {
      commit("RESET_FILTER");
      commit("FILTER_ALBUM_SET", albumId);
      dispatch("search");
    },
    setGeoFilter({ dispatch, commit }, geo) {
      commit("RESET_FILTER");
      commit("FILTER_GEO_SET", geo);
      dispatch("search");
    },
    async loadDetails({ commit }, id) {
      try {
        const res = await getById(id);
        commit("DETAILS_LOADED", res.data.mediaById);
      } catch (ex) {
        console.error(ex);
      }
    },
    async getSearchFacets({ commit }) {
      try {
        const res = await getSearchFacets();
        commit("SEARCH_FACETS_LOADED", res.data.facets);
      } catch (ex) {
        console.error(ex);
      }
    },
    toggleUploadDialog: function ({ commit }, open) {
      commit("UPLOAD_DIALOG_TOGGLED", open);
    },
    toggleEditMode: function ({ commit }, value) {
      commit("EDIT_MODE_TOGGLE", value);
    },
    select: function ({ commit }, id) {
      commit("SELECTED", id);
    },
    selectAll: function ({ commit }) {
      commit("ALL_SELECTED");
    },
    clearSelected: function ({ commit }) {
      commit("CLEAR_SELECTED");
    },
    async toggleFavorite({ commit }, media) {
      await toggleFavorite(media.id, !media.isFavorite);
      commit("FAVORITE_TOGGLED", media);
    }
  },
  getters: {
    next: state => step => {
      const currentId = state.current.id;
      const idx = state.list.findIndex(x => x.id == currentId);
      if (idx > -1) {
        const newIndex = idx + step;
        if (newIndex > state.list.length) return null;
        return state.list[newIndex].id;
      }

      return null;
    },
    selectedMediaIds: state => {
      return getMediaIdsFromIndexes(state);
    }
  }
};

export default mediaModule;
