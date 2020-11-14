import Vue from "vue";

import { createGroup, getAllPersons, updatePerson, getAllGroups } from "../services/personService";

const personModule = {
  namespaced: true,
  state: () => ({
    persons: [],
    groups: [],
    filter: {
      groups: [],
      searchText: ''
    }
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
    },
    GROUPS_LOADED(state, groups) {
      Vue.set(state, "groups", [...groups]);
    },
    GROUP_ADDED(state, group) {
      var groups = state.groups.filter(x => x.id == group.id);
      if (groups.length === 0) state.groups.push(group);
    },
    FILTER_SET(state, filter) {
      state.filter = filter;
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
    async getAllGroups({ commit }) {
      try {
        const res = await getAllGroups();
        commit("GROUPS_LOADED", res.data.groups);
      } catch (ex) {
        console.error(ex);
      }
    },
    async update({ commit }, input) {
      try {
        const res = await updatePerson(input);
        commit("PERSON_UPDATED", res.data.updatePerson.person);

        if (input.newGroups.length > 0) {
          res.data.updatePerson.person.groups.forEach(group => {
            commit('GROUP_ADDED', group)
          });
        }

      } catch (ex) {
        console.error(ex);
      }
    },
    async addGroup({ commit }, name) {
      try {
        const res = await createGroup(name);
        commit("GROUP_ADDED", res.data.createGroup.group);
      } catch (ex) {
        console.error(ex);
      }
    },
    filter: function ({ commit }, filter) {
      commit("FILTER_SET", filter)
    }
  },
  getters: {}
};

export default personModule;
