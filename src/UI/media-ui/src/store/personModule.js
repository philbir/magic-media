import Vue from "vue";
import { createGroup, getAllPersons, updatePerson, getAllGroups, search, buildModel, deletePerson } from "../services/personService";
import { excuteGraphQL } from "./graphqlClient"
import { addSnack } from "./snackService"

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
    async search({ commit, state, dispatch }) {
      commit("SET_SEARCH_LOADING", true);
      const result = await excuteGraphQL(() => search(state.filter), dispatch);

      if (result.success) {
        commit("SEARCH_COMPLETED", result.data.searchPersons);

      }
      commit("SET_SEARCH_LOADING", false);
    },
    async getAll({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getAllPersons(), dispatch);

      if (result.success) {
        commit("PERSONS_LOADED", result.data.persons);
      }
    },
    async getAllGroups({ commit, dispatch }) {
      const result = await excuteGraphQL(() => getAllGroups(), dispatch);

      if (result.success) {
        commit("GROUPS_LOADED", result.data.groups);
      }
    },
    async update({ commit, dispatch }, input) {
      const result = await excuteGraphQL(() => updatePerson(input), dispatch);

      if (result.success) {
        commit("PERSON_UPDATED", result.data.updatePerson.person);

        if (input.newGroups.length > 0) {
          result.data.updatePerson.person.groups.forEach(group => {
            commit('GROUP_ADDED', group)
          });
        }
        addSnack(dispatch, `${result.data.updatePerson.person.name} saved.`);

      }
    },
    async addGroup({ commit, dispatch }, name) {
      const result = await excuteGraphQL(() => createGroup(name), dispatch);

      if (result.success) {
        commit("GROUP_ADDED", result.data.createGroup.group);
      }
    },
    async delete({ dispatch }, id) {
      const result = await excuteGraphQL(() => deletePerson(id), dispatch);

      if (result.success) {
        addSnack(dispatch, 'Person deleted');
      }
    },
    async buildModel({ dispatch }) {
      const result = await excuteGraphQL(() => buildModel(), dispatch);

      if (result.success) {
        addSnack(dispatch, `Person model build. (${result.data.buildPersonModel.faceCount} faces)`)
      }
    },
    filter: function ({ commit, dispatch }, filter) {
      commit("FILTER_SET", filter)
      dispatch('search');
    }
  },
  getters: {

  }
};

export default personModule;
