import Vue from "vue";
import Vuex from "vuex";

import { getAllPersons } from "../services/personService";
import mediaModule from "./mediaModule";
import personModule from "./personModule";
import snackbarModule from "./snackbarModule";

/* eslint-disable no-debugger */

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    persons: [],
  },
  mutations: {
    PERSONS_LOADED(state, persons) {
      Vue.set(state, "persons", [...persons]);
    },
    PERSON_ADDED(state, person) {
      var persons = state.persons.filter(x => x.id == person.id);
      if (persons.length === 0) state.persons.push(person);
    }

  },
  actions: {
    async getAllPersons({ commit }) {
      try {
        const res = await getAllPersons();
        commit("PERSONS_LOADED", res.data.persons);
      } catch (ex) {
        console.error(ex);
      }
    }
  },
  getters: {

  },
  modules: {
    snackbar: snackbarModule,
    media: mediaModule,
    person: personModule
  }
});
