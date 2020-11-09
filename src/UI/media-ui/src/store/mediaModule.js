import Vue from "vue";

import {
  getById,
  getFolderTree,
  getSearchFacets,
  moveMedia,
  searchMedia
} from "../services/mediaService";
/* eslint-disable no-debugger */


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
    totalCount: 0,
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
      persons: []
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
      state.totalCount = result.totalCount;
      state.totalLoaded = state.totalLoaded + result.items.length;
      state.hasMore = result.items.length > 0;
    },
    DETAILS_LOADED(state, media) {
      state.currentMediaId = media.id;
      state.current = Object.assign({}, media);
    },
    FOLDER_TREE_LOADED(state, tree) {
      console.log(tree);
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
    PAGE_NR_INC(state) {
      state.filter.pageNr++;
    },
    UPLOAD_DIALOG_TOGGLED: function(state, open) {
      state.uploadDialog.open = open;
    },
    SET_MEDIALIST_LOADING: function(state, isloading) {
      state.listLoading = isloading;
    },
    SEARCH_FACETS_LOADED: function(state, facets) {
      Vue.set(state, "facets", facets);
    },
    RESET_FILTER: function(state) {
      state.list = [];
      state.filter.pageNr = 0;
      state.totalLoaded = 0;
      state.totalCount = 0;
      state.selectedIndexes = [];
    },
    MEDIA_CLOSED: function(state) {
      state.currentMediaId = null;
      state.current = null;
    },
    EDIT_MODE_TOGGLE: function(state, value) {
      state.isEditMode = value;
      if (!value) {
        state.selectedIndexes = [];
      }
    },
    SELECTED: function(state, idx) {
      const current = [...state.selectedIndexes];
      const i = current.indexOf(idx);
      if (i > -1) {
        current.splice(i, 1);
      } else {
        current.push(idx);
      }

      Vue.set(state, "selectedIndexes", current);
    },
    ALL_SELECTED: function(state) {
      state.selectedIndexes = [...Array(state.list.length).keys()];
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
    async moveSelected({ commit, state }, newLocation) {
      try {
        const ids = [];
        for (let i = 0; i < state.selectedIndexes.length; i++) {
          ids.push(state.list[i].id);
        }

        const res = await moveMedia({
          ids,
          newLocation
        });

        commit("OPERATION_COMMITED", res.moveMedia.id);
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
    toggleUploadDialog: function({ commit }, open) {
      commit("UPLOAD_DIALOG_TOGGLED", open);
    },
    toggleEditMode: function({ commit }, value) {
      commit("EDIT_MODE_TOGGLE", value);
    },
    select: function({ commit }, id) {
      commit("SELECTED", id);
    },
    selectAll: function({ commit }) {
      commit("ALL_SELECTED");
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
    }
  }
};

export default mediaModule;
