import Vue from "vue";

import {
  approveAllByMedia,
  approveFace,
  assignPerson,
  deleteFace,
  deleteUnassignedByMedia,
  predictPerson,
  predictPersonsByMedia,
  searchFaces,
  unAssignAllPrecictedByMedia,
  unAssignPerson
} from "../services/faceService";

import { excuteGraphQL } from "./graphqlClient"

const faceModule = {
  namespaced: true,
  state: () => ({
    list: [],
    listLoading: false,
    totalLoaded: 0,
    totalCount: 0,
    selectedIndexes: [],
    lastSelectedIndex: -1,
    isEditMode: false,
    hasMore: true,
    filter: {
      pageNr: 0,
      pageSize: 200,
      persons: [],
      states: [],
      recognitionTypes: []
    },
    editFaceId: null,
    editFace: null
  }),
  mutations: {
    LIST_LOADED(state, result) {
      const max = 600;
      const current = [...state.list];
      if (current.length > max) {
        current.splice(0, state.filter.pageSize);
      }
      Vue.set(state, "list", [...current, ...result.items]);
      state.listLoading = false;
      state.totalCount = result.totalCount;

      Vue.set(state, "totalLoaded", state.totalLoaded + result.items.length);
      state.hasMore = result.totalCount > state.totalLoaded;
    },
    FILTER_PERSONS_SET(state, persons) {
      state.filter.persons = persons;
    },
    FILTER_RECOGNITIONTYPE_SET(state, types) {
      state.filter.recognitionTypes = types;
    },
    FILTER_STATE_SET(state, states) {
      state.filter.states = states;
    },
    PAGE_NR_INC(state) {
      state.filter.pageNr++;
    },
    RESET_FILTER: function (state) {
      state.list = [];
      state.filter.pageNr = 0;
      state.totalLoaded = 0;
      state.selectedIndexes = [];
    },
    SET_LIST_LOADING: function (state, isloading) {
      state.listLoading = isloading;
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
    EDIT_MODE_TOGGLE: function (state, value) {
      state.isEditMode = value;
      if (!value) {
        state.selectedIndexes = [];
        state.lastSelectedIndex = -1;
      }
    },
    ALL_SELECTED: function (state) {
      state.selectedIndexes = [...Array(state.list.length).keys()];
    },
    FACE_EDIT_OPENED: function (state, face) {
      state.editFaceId = face.id;
      state.editFace = face;
    },
    FACE_EDIT_CLOSED: function (state) {
      state.editFaceId = null;
      state.editFace = null;
    },
    FACE_UPDATED: function (state, face) {
      state.editFace = face;

      const listIdx = state.list.findIndex(x => x.id == face.id);
      if (listIdx > -1) {
        state.list[listIdx] = face;
      }
    },
    FACE_DELETED: function (state, id) {
      const listIdx = state.list.findIndex(x => x.id == id);
      if (listIdx > -1) {
        state.list.splice(listIdx, 1);
      }
    }
  },
  actions: {
    async search({ commit, state, dispatch }) {
      commit("SET_LIST_LOADING", true);
      const result = await excuteGraphQL(() => searchFaces(state.filter), dispatch);

      if (result.success) {
        commit("LIST_LOADED", result.data.searchFaces);
      }
      else {
        commit("SET_LIST_LOADING", false);
      }

    },
    async loadMore({ commit, state, dispatch }) {
      if (state.hasMore) {
        commit("PAGE_NR_INC");
        dispatch("search");
      }
    },
    setPersonFilter({ dispatch, commit }, persons) {
      commit("RESET_FILTER");
      commit("FILTER_PERSONS_SET", persons);
      dispatch("search");
    },
    setRecognitionTypeFilter({ dispatch, commit }, types) {
      commit("RESET_FILTER");
      commit("FILTER_RECOGNITIONTYPE_SET", types);
      dispatch("search");
    },
    setStateFilter({ dispatch, commit }, states) {
      commit("RESET_FILTER");
      commit("FILTER_STATE_SET", states);
      dispatch("search");
    },
    toggleEditMode: function ({ commit }, value) {
      commit("EDIT_MODE_TOGGLE", value);
    },
    select: function ({ commit }, payload) {
      commit("SELECTED", payload);
    },
    selectAll: function ({ commit }) {
      commit("ALL_SELECTED");
    },
    openEdit: function ({ commit }, face) {
      commit("FACE_EDIT_OPENED", face);
    },
    closeEdit: function ({ commit }) {
      commit("FACE_EDIT_CLOSED");
    },
    async setName({ commit, dispatch }, data) {

      const result = excuteGraphQL(() => assignPerson(data.id, data.name), dispatch);

      if (result.success) {
        const { face } = result.data.assignPersonByHuman;

        commit("FACE_UPDATED", face);
        commit("person/PERSON_ADDED", face.person, { root: true });

        dispatch("media/faceUpdated", face, {
          root: true
        });
      }
    },
    async unassignPerson({ commit, dispatch }, id) {

      const result = await excuteGraphQL(() => unAssignPerson(id), dispatch);

      if (result.success) {
        const { face } = result.data.unAssignPersonFromFace;

        commit("FACE_UPDATED", face);
        dispatch("media/faceUpdated", face, {
          root: true
        });
      }

    },
    async approve({ commit, dispatch }, id) {

      const result = await excuteGraphQL(() => approveFace(id), dispatch);

      if (result.success) {
        const { face } = result.data.approveFaceComputer;

        commit("FACE_UPDATED", face);
        dispatch("media/faceUpdated", face, {
          root: true
        });
      }
    },
    async approveAllByMedia({ commit, dispatch }, mediaId) {
      const result = await excuteGraphQL(() => approveAllByMedia(mediaId), dispatch);

      if (result.success) {
        const { faces } = result.data.approveAllFacesByMedia;

        faces.forEach(face => {
          commit("FACE_UPDATED", face);
        });

        //TODO: Better patch currentDetails
        dispatch("media/loadDetails", mediaId, {
          root: true
        });
      }
    },
    async unAssignPredictedByMedia({ commit, dispatch }, mediaId) {
      const result = await excuteGraphQL(() => unAssignAllPrecictedByMedia(mediaId), dispatch);

      if (result.success) {
        const { faces } = result.data.unAssignAllPredictedPersonsByMedia;

        faces.forEach(face => {
          commit("FACE_UPDATED", face);
        });

        //TODO: Better patch currentDetails
        dispatch("media/loadDetails", mediaId, {
          root: true
        });
      }

    },
    async predictPerson({ commit, dispatch }, id) {
      const result = await excuteGraphQL(() => predictPerson(id), dispatch);

      if (result.success) {
        const { face, hasMatch } = result.data.predictPerson;

        commit("FACE_UPDATED", face);
        dispatch("media/faceUpdated", face, {
          root: true
        });

        if (hasMatch) {
          dispatch(
            "snackbar/addSnack",
            { text: face.person.name + " found.", type: "SUCCESS" },
            { root: true }
          );
        } else {
          dispatch(
            "snackbar/addSnack",
            { text: "No person found.", type: "INFO" },
            { root: true }
          );
        }
      }
    },
    async predictPersonsByMedia({ dispatch }, mediaId) {

      const result = await excuteGraphQL(() => predictPersonsByMedia(mediaId), dispatch);

      if (result.success) {
        const { media, matchCount } = result.data.predictPersonsByMedia;

        //TODO: Better patch currentDetails
        dispatch("media/loadDetails", media.id, {
          root: true
        });

        if (matchCount > 0) {
          dispatch(
            "snackbar/addSnack",
            { text: `${matchCount} persons found.`, type: "SUCCESS" },
            { root: true }
          );
        } else {
          dispatch(
            "snackbar/addSnack",
            { text: "No persons found.", type: "INFO" },
            { root: true }
          );
        }
      }
    },
    async deleteFace({ commit, dispatch }, face) {
      const result = await excuteGraphQL(() => deleteFace(face.id), dispatch);

      if (result.success) {
        commit("FACE_DELETED", face.id);
        dispatch(
          "media/faceUpdated",
          { mediaId: face.mediaId },
          {
            root: true
          }
        );
      }

    },
    async deleteUnassignedByMedia({ commit, dispatch }, mediaId) {
      const result = await excuteGraphQL(() => deleteUnassignedByMedia(mediaId), dispatch);

      if (result.success) {
        const faceIds = result.data.deleteUnassignedFacesByMedia.ids;

        faceIds.forEach(id => {
          commit("FACE_DELETED", id);
        });

        //TODO: Better patch currentDetails
        dispatch("media/loadDetails", mediaId, {
          root: true
        });
      }
    }
  },
  getters: {
    next: (state, getters, rootState) => step => {


      const currentId = rootState.media.current.id;
      const idx = state.list.findIndex(x => x.media.id == currentId);
      let newIndex = idx;
      if (idx > -1) {
        let newId = null;
        while (newId === null) {
          newIndex = newIndex + step;

          if (newIndex < 0 || newIndex > state.list.length) {
            break;
          }
          if (currentId !== state.list[newIndex].media.id) {
            newId = state.list[newIndex].media.id;
          }
        }

        return newId;
      }
    }
  }
};

export default faceModule;
