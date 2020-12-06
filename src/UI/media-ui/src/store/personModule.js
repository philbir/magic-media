import Vue from "vue";

import { createGroup, getAllPersons, updatePerson, getAllGroups, search, buildModel } from "../services/personService";

const personModule = {
  namespaced: true,
  state: () => ({
    persons: [],
    groups: [],
    hasMore: true,
    listLoading: false,
    totalCount: 0,
    list: [],
    filter: {
      pageNr: 0,
      pageSize: 50,
      groups: [],
      searchText: ''
    }
  }),
  mutations: {
    SEARCH_COMPLETED(state, result) {

      state.listLoading = false;
      state.totalCount = result.totalCount;

      state.hasMore = result.totalCount > state.totalLoaded;
      Vue.set(state, "list", [...result.items]);

    },
    SET_SEARCH_LOADING(state, loading) {
      state.listLoading = loading;
    },
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

      state.filter = Object.assign(state.filter, filter);

    }
  },
  actions: {
    async search({ commit, state }) {
      try {
        commit("SET_SEARCH_LOADING", true);
        const res = await search(state.filter);
        commit("SEARCH_COMPLETED", res.data.searchPersons);
      } catch (ex) {
        this.$magic.snack("Error loading", "ERROR");
      }
    },
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
    async buildModel({ dispatch }) {
      try {
        const res = await buildModel();

        dispatch(
          "snackbar/addSnack",
          {
            text: `Person model build. (${res.data.buildPersonModel.faceCount} faces)`,
            type: "SUCCESS"
          },
          { root: true }
        );

      } catch (ex) {
        console.error(ex);
      }
    },
    filter: function ({ commit, dispatch }, filter) {
      commit("FILTER_SET", filter)
      dispatch('search');

    }
  },
  getters: {}
};

export default personModule;
