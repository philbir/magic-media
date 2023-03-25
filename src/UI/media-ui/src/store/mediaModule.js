import Vue from "vue";
import { v4 as uuidv4 } from 'uuid';

import {
  analyseMedia,
  deleteMedia,
  getById,
  getFolderTree,
  getSearchFacets,
  moveMedia,
  recycleMedia,
  searchMedia,
  toggleFavorite,
  updateMetadata,
  reScanFaces
} from "../services/mediaService";

import MediaFilterManager from '../services/mediaFilterManager';
import { excuteGraphQL } from "./graphqlClient"
import { mediaOperationTypeMap } from "../services/mediaOperationService";
import { shareManyMedia } from "../services/shareService"
import { exportMedia } from "../services/mediaService";
import { quickExportMedia } from "../services/mediaService";
import { addSnack } from "./snackService"
import { mediaListViewMap } from "../services/mediaListViewMap";

const getMediaIdsFromIndexes = state => {
  const ids = [];
  for (let i = 0; i < state.selectedIndexes.length; i++) {
    ids.push(state.list[state.selectedIndexes[i]].id);
  }
  return ids;
};

const getSelectedMedias = state => {
  const medias = [];
  for (let i = 0; i < state.selectedIndexes.length; i++) {
    medias.push(state.list[state.selectedIndexes[i]]);
  }

  return medias;
}

const fm = new MediaFilterManager();

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
    viewerHeaderLoading: false,
    current: null,
    totalLoaded: 0,
    currentMediaId: null,
    listLoading: false,
    selectedIndexes: [],
    lastSelectedIndex: -1,
    hasMore: true,
    isEditMode: false,
    thumbnailSize: "M",
    loadThumbnailData: false,
    filter: {
      pageNr: 0,
      pageSize: 200,
      countries: [],
      cities: [],
      persons: [],
      groups: [],
      tags: [],
      aiTags: [],
      objects: [],
      mediaTypes: [],
      cameras: [],
      date: null,
      albumId: null,
      geoRadius: null,
      folder: window.location.href.startsWith("http://local") ?  "00_LocalDev" : null,
      text: null
    },
    recentMoves: [],
    viewer: {
      showFaceBox: true,
      showFaceList: true,
      showFilmStripe: false,
      showObjects: false
    }
  }),
  mutations: {
    MOBILE_DETECTED(state, mobile) {
      if (mobile) {
        state.viewer.showFaceBox = false;
        state.viewer.showFaceList = false;
        state.thumbnailSize = 'SQ_S'
      }
    },
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
      state.viewerHeaderLoading = false
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
    FILTER_SET(state, filter) {
      fm.setFilter(state, filter.key, filter.value);
    },
    FILTER_REMOVED(state, key) {
      fm.removeFilter(state, key);
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
      state.filter.tags = [];
      state.filter.objects = [];
      state.filter.groups = [];
      state.filter.persons = [];
      state.filter.mediaTypes = [];
      state.filter.cameras = [];
      state.filter.albumId = null;
      state.filter.geoRadius = null;
      state.filter.folder = null;
      state.filter.date = null;
    },
    LOAD_TUMBNAIL_DATA_TOGGLED: function (state) {
      state.loadThumbnailData = !state.loadThumbnailData;
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
    SET_HEADER_LOADING: function (state, loading) {
      state.viewerHeaderLoading = loading;
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
    OPERATION_COMMITED: function (state, payload) {
      var mediaIds = getMediaIdsFromIndexes(state);
      const current = [...state.list];
      var operationType = mediaOperationTypeMap[payload.type];

      if (operationType.removeFromList) {
        for (let i = 0; i < mediaIds.length; i++) {
          var idx = current.findIndex(x => x.id === mediaIds[i]);
          current.splice(idx, 1);
        }
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
    async search({ commit, state, dispatch }) {
      commit("SET_MEDIALIST_LOADING", true);

      const viewMap = mediaListViewMap[state.thumbnailSize];

      const result = await excuteGraphQL(() => searchMedia(
        state.filter,
        viewMap.thumbSize,
        state.loadThumbnailData), dispatch);

      if (result.success) {

        commit("MEDIAITEMS_LOADED", result.data.searchMedia);
      }
      else {
        commit("SET_MEDIALIST_LOADING", false);
      }
    },
    async loadMore({ commit, state, dispatch }) {
      if (state.hasMore) {
        commit("PAGE_NR_INC");
        dispatch("search");
      }
    },
    async show({ commit, dispatch }, id) {
      commit('SET_HEADER_LOADING', true)
      const result = await excuteGraphQL(() => getById(id), dispatch);

      if (result.success) {
        commit("DETAILS_LOADED", result.data.mediaById);
      }
    },
    async getFolderTree({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getFolderTree(), dispatch);

      if (result.success) {
        commit("FOLDER_TREE_LOADED", result.data.folderTree);
      }

    },
    async startOperation({ commit, dispatch }, operation) {
      const result = await excuteGraphQL(() => operation.api, dispatch);

      if (result.success) {
        const operationId = result.data[operation.dataField].operationId;

        commit("OPERATION_COMMITED", { id: operationId, type: operation.type });

        dispatch(
          "snackbar/operationStarted",
          {
            id: operationId,
            operationType: operation.type,
            type: "INFO",
            title: mediaOperationTypeMap[operation.type].title,
            totalCount: operation.ids.length,
            text: mediaOperationTypeMap[operation.type].title
          },
          { root: true }
        );
      }
    },
    async moveSelected({ state, dispatch, getters }, request) {
      if (!getters["canEdit"])
        return;

      let ids = [];
      if (state.currentMediaId) {
        ids.push(state.currentMediaId)
      }
      else {
        ids = getMediaIdsFromIndexes(state);
      }

      const operation = {
        type: 0,
        api: moveMedia({
          ids,
          newLocation: request.newLocation,
          rule: request.rule,
          operationId: uuidv4()
        }),
        dataField: "moveMedia",
        ids: ids,
      }

      const recents = state.recentMoves.filter(x => x == request.newLocation);
      if (recents.length === 0) {

        if (state.recentMoves.length > 3) {
          state.recentAlbums.splice(0, 1);
        }
        state.recentMoves.push(request.newLocation);
      }

      dispatch('startOperation', operation);
    },
    async recycle({ state, dispatch, getters }, ids) {

      if (!getters["canEdit"])
        return;

      ids = ids ?? getMediaIdsFromIndexes(state);
      const operation = {
        type: 1,
        api: recycleMedia({
          ids,
          operationId: uuidv4()
        }),
        dataField: "recycleMedia",
        ids: ids,
      }
      dispatch('startOperation', operation);

    },
    async delete({ state, dispatch, getters }, ids) {
      if (!getters["canEdit"])
        return;

      ids = ids ?? getMediaIdsFromIndexes(state);

      const operation = {
        type: 4,
        api: deleteMedia({
          ids,
          operationId: uuidv4()
        }),
        dataField: "deleteMedia",
        ids: ids,
      }

      dispatch('startOperation', operation);
    },
    async exportSelected({ state, dispatch, getters }, request) {
      if (!getters["canEdit"])
        return;

      const ids = getMediaIdsFromIndexes(state);

      const operation = {
        type: 5,
        api: exportMedia({
          ids: ids,
          profileId: request.profileId,
          path: request.path,
          operationId: uuidv4()
        }),
        dataField: "exportMedia",
        ids: ids,
      }

      dispatch('startOperation', operation);
    },
    async reScanFaces({ state, dispatch, getters }) {
      if (!getters["canEdit"])
        return;

      const ids = getMediaIdsFromIndexes(state);

      const operation = {
        type: 3,
        api: reScanFaces({
          ids: ids,
          operationId: uuidv4()
        }),
        dataField: "reScanFaces",
        ids: ids,
      }

      dispatch('startOperation', operation);
    },
    async updateMetadata({ dispatch, getters }, input) {

      if (!getters["canEdit"])
        return;

      const operation = {
        type: 2,
        api: updateMetadata(input),
        dataField: "updateMediaMetadata",
        ids: input.ids,
      }
      dispatch('startOperation', operation);
    },
    async share({ dispatch }, medias) {

      if (!navigator.share) {
        addSnack(dispatch, `Sharing is not possible on this device`, "ERROR");
        return;
      }

      if (medias.length > 10) {
        addSnack(dispatch, `You can not share more then 10 items at the same time`, "WARN");
        return;
      }

      addSnack(dispatch, `Prepare ${medias.length} images for sharing.`, "INFO");
      try {
        await shareManyMedia(medias);
        addSnack(dispatch, `Sharing completed.`);

      }
      catch {
        addSnack(dispatch, `Error while sharing`, "ERROR");
      }
    },
    async quickExport({ dispatch }, id) {

      addSnack(dispatch, 'Quick export started...', "INFO");
      const result = await quickExportMedia({ id: id });
      addSnack(dispatch, `Exported to ${result?.data?.quickExportMedia.path}`);
    },
    async shareSelected({ state, dispatch }) {

      const medias = getSelectedMedias(state);
      await dispatch('share', medias);

    },
    close({ commit }) {
      commit("MEDIA_CLOSED");
    },
    setThumbnailSize({ dispatch, commit }, size) {
      commit("FILTER_THUMBNAIL_SIZE_SET", size);
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
    setFilter({ dispatch, commit }, filter) {
      commit("RESET_FILTER");
      commit("FILTER_SET", filter);
      dispatch("search");
    },
    viewAlbum({ dispatch, commit }, albumId) {
      commit("RESET_FILTER");
      commit("RESET_FILTER_VALUES");
      commit("FILTER_SET", { key: 'albumId', value: albumId });
      dispatch("search");
    },
    removeFilter({ dispatch, commit }, key) {
      commit("RESET_FILTER");
      commit("FILTER_REMOVED", key);
      dispatch("search");
    },
    toggleLoadThumbnailData({ dispatch, commit }) {
      commit("RESET_FILTER");
      commit("LOAD_TUMBNAIL_DATA_TOGGLED");
      dispatch("search");

    },
    async loadDetails({ commit, dispatch }, id) {

      const result = await excuteGraphQL(() => getById(id), dispatch);

      if (result.success) {
        commit("DETAILS_LOADED", result.data.mediaById);
      }

    },
    async getSearchFacets({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getSearchFacets(), dispatch);

      if (result.success) {
        commit("SEARCH_FACETS_LOADED", result.data.facets);
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
    async toggleFavorite({ commit, getters, dispatch }, media) {
      if (!getters["canEdit"])
        return;

      const result = await excuteGraphQL(() => toggleFavorite(media.id, !media.isFavorite), dispatch);

      if (result.success) {
        commit("FAVORITE_TOGGLED", media);
      }
    },
    async analyseAI({ dispatch, getters, commit }, id) {
      if (!getters["canEdit"])
        return;

      commit('SET_HEADER_LOADING', true);
      const result = await excuteGraphQL(() => analyseMedia(id), dispatch);

      if (result.success) {
        dispatch('loadDetails', id);

        dispatch(
          "snackbar/addSnack",
          { text: "Cloud AI analyze completed.", type: "SUCCESS" },
          { root: true }
        );
      }

      commit('SET_HEADER_LOADING', false);

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
    },
    canEdit: (state, getters, rootState, rootGetters) => {
      return rootGetters["user/userActions"].media.edit;
    },
    filterDescriptions: (state, getters, rootState) => {

      if (state.facets) {
        return fm.buildDescriptions(rootState, state, state.filter.folder);
      }
      return [];
    }
  },

};

export default mediaModule;
