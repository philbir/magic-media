import Vue from "vue";

import { getAllPersons, updatePerson } from "../services/personService";

const personModule = {
  namespaced: true,
  state: () => ({
    persons: []
  }),
  mutations: {
    PERSONS_LOADED(state, persons) {
      Vue.set(state, "persons", [...persons]);
    },
    PERSON_ADDED(state, person) {
      var persons = state.persons.filter(x => x.id == person.id);
      if (persons.length === 0) state.persons.push(person);
    },
    PERSON_UPDATED(state, person) {
      var idx = state.persons.findIndex(x => x.id == person.id);
      state.persons[idx] = { ...person };
    }
  },
  actions: {
    async getAll({ commit }) {
      try {
        const res = await getAllPersons();
        commit("PERSONS_LOADED", res.data.persons);
      } catch (ex) {
        console.error(ex);
      }
    },
    async update({ commit }, input) {
      try {
        const res = await updatePerson(input);
        commit("PERSON_UPDATED", res.data.updatePerson.person);
      } catch (ex) {
        console.error(ex);
      }
    }
  },
  getters: {}
};

export default personModule;
