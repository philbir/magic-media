import Vue from "vue";
import Vuex from "vuex";

import { getById, searchMedia } from "../services/mediaService";
import { getAllPersons } from "../services/personService";

/* eslint-disable no-debugger */

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    mediaList: [],
    currentMedia: null,
    persons: []
  },
  mutations: {
    MEDIAITEMS_LOADED(state, mediaList) {
      Vue.set(state, "mediaList", [...mediaList]);
    },
    MEDIADETAILS_LOADED(state, media) {
      state.currentMedia = Object.assign({}, media);
    },
    PERSONS_LOADED(state, persons) {
      Vue.set(state, "persons", [...persons]);
    },
    PERSON_ADDED(state, person) {
      var persons = state.persons.filter(x => x.id == person.id);
      if (persons.length === 0) state.persons.push(person);
    }
  },
  actions: {
    async searchMedia({ commit }) {
      try {
        const res = await searchMedia();
        commit("MEDIAITEMS_LOADED", res.data.searchMedia);
      } catch (ex) {
        console.error(ex);
      }
    },
    async loadMediaDetails({ commit }, id) {
      try {
        const res = await getById(id);
        commit("MEDIADETAILS_LOADED", res.data.mediaById);
      } catch (ex) {
        console.error(ex);
      }
    },
    async getAllPersons({ commit }) {
      try {
        const res = await getAllPersons();
        console.log(res.data);
        commit("PERSONS_LOADED", res.data.persons);
      } catch (ex) {
        console.error(ex);
      }
    }
  },
  getters: {
    nextMedia: state => step => {
      const currentId = state.currentMedia.id;
      const idx = state.mediaList.findIndex(x => x.id == currentId);
      if (idx > -1) {
        const newIndex = idx + step;
        if (newIndex > state.mediaList.length) return null;
        return state.mediaList[newIndex];
      }

      return null;
    }
  },
  modules: {}
});
