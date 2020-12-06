import Vue from "vue";

import {
  deleteMedia,
  getById,
  getFolderTree,
  getSearchFacets,
  moveMedia,
  recycleMedia,
  searchMedia,
  toggleFavorite,
  updateMetadata
} from "../services/mediaService";

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
    lastSelectedIndex: -1,
    hasMore: true,
    isEditMode: false,
    thumbnailSize: "M",
    filter: {
      pageNr: 0,
      pageSize: 200,
      countries: [],
      cities: [],
      persons: [],
      mediaTypes: [],
      cameras: [],
      date: null,
      albumId: null,
      geoRadius: null,
      folder: null
    },
    viewer: {
      showFaceBox: true,
      showFaceList: true,
      showFilmStripe: false,
      showObjects: false
    }
  }),
  mutations: {
    MEDIAITEMS_LOADED(state, result) {
      const max = 1000;
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
      state.totalLoaded = 0;
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
    FILTER_CAMERA_SET(state, ids) {
      state.filter.cameras = ids;
    },
    FILTER_MEDIATYPES_SET(state, types) {
      state.filter.mediaTypes = types;
    },
    FILTER_GEO_SET(state, geo) {
      state.filter.geoRadius = geo;
    },
    FILTER_DATE_SET(state, date) {
      state.filter.date = date;
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
    RESET_FILTER_VALUES: function (state) {
      state.filter.countries = [];
      state.filter.cities = [];
      state.filter.persons = [];
      state.filter.mediaTypes = [];
      state.filter.cameras = [];
      state.filter.albumId = null;
      state.filter.geoRadius = null;
      state.filter.folder = null;
      state.filter.date = null;
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
    SELECTED: function (state, payload) {
      const { idx, multi } = payload;
      const current = [...state.selectedIndexes];
      const isSelected = state.selectedIndexes.includes(idx);
      if (state.lastSelectedIndex > -1 && multi) {
        if (idx > state.lastSelectedIndex) {
          for (let i = state.lastSelectedIndex; i <= idx; i++) {
            if (!isSelected) {
              current.push(i);
            }

          }
        } else {
          for (let i = idx; i <= state.lastSelectedIndex; i++) {
            if (!isSelected) {
              current.push(i);
            }

          }
        }
      } else {

        const i = current.indexOf(idx);
        if (i > -1) {
          current.splice(i, 1);
        } else {
          current.push(idx);
        }
      }
      state.lastSelectedIndex = idx;
      Vue.set(state, "selectedIndexes", current);
    },
    ALL_SELECTED: function (state) {
      state.selectedIndexes = [...Array(state.list.length).keys()];
    },
    CLEAR_SELECTED: function (state) {
      state.selectedIndexes = [];
      state.lastSelectedIndex = -1;
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
    },
    VIEWER_OPTIONS_SET: function (state, options) {
      state.viewer = options;
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
    async recycle({ commit, state, dispatch }, ids) {
      try {
        ids = ids ?? getMediaIdsFromIndexes(state);
        const res = await recycleMedia({
          ids
        });

        commit("OPERATION_COMMITED", res.data.recycleMedia.operationId);

        dispatch(
          "snackbar/operationStarted",
          {
            id: res.data.recycleMedia.operationId,
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
    async delete({ commit, state, dispatch }, ids) {
      try {
        ids = ids ?? getMediaIdsFromIndexes(state);
        const res = await deleteMedia({
          ids
        });

        commit("OPERATION_COMMITED", res.data.deleteMedia.operationId);

        dispatch(
          "snackbar/operationStarted",
          {
            id: res.data.deleteMedia.operationId,
            type: "INFO",
            title: "Delete media",
            totalCount: ids.length,
            text: "Delete media"
          },
          { root: true }
        );
      } catch (ex) {
        console.error(ex);
        this.$magic.snack("Error loading", "ERROR");
      }
    },
    async updateMetadata({ commit, dispatch }, input) {
      try {
        const res = await updateMetadata(input);

        commit("OPERATION_COMMITED", res.data.updateMediaMetadata.operationId);

        dispatch(
          "snackbar/operationStarted",
          {
            id: res.data.updateMediaMetadata.operationId,
            type: "INFO",
            title: "Update metadata",
            totalCount: input.ids.length,
            text: "Update metadata"
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
    setMediaTypeFilter({ dispatch, commit }, types) {
      commit("RESET_FILTER");
      commit("FILTER_MEDIATYPES_SET", types);
      dispatch("search");
    },
    setCamaraFilter({ dispatch, commit }, cameras) {
      commit("RESET_FILTER");
      commit("FILTER_CAMERA_SET", cameras);
      dispatch("search");
    },
    setDateFilter({ dispatch, commit }, date) {
      commit("RESET_FILTER");
      commit("FILTER_DATE_SET", date);
      dispatch("search");
    },
    setViewerOptions({ commit }, options) {
      commit("VIEWER_OPTIONS_SET", options);
    },
    resetAllFilters({ dispatch, commit }) {
      commit("RESET_FILTER_VALUES");
      commit("RESET_FILTER");
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
    toggleEditMode: function ({ commit, state }, value) {

      if (value === undefined) {
        value = !state.isEditMode
      }

      commit("EDIT_MODE_TOGGLE", value);
    },
    select: function ({ commit }, payload) {
      commit("SELECTED", payload);
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
    },
    faceUpdated: function ({ dispatch }, face) {
      //TODO: Patch details instead of reloading...
      dispatch("loadDetails", face.mediaId);
    }
  },
  getters: {
    next: state => step => {
      const currentId = state.current.id;
      const idx = state.list.findIndex(x => x.id == currentId);

      if (idx > -1) {
        const newIndex = idx + step;
        if (typeof state.list[newIndex] === 'undefined') {
          return null;
        }
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
