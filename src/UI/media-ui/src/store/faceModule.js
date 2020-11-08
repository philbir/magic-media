import Vue from 'vue';

import { searchFaces } from "../services/faceService";
/* eslint-disable no-debugger */

const faceModule = {
    namespaced: true,
    state: () => ({
        list: [],
        listLoading: false,
        totalLoaded: 0,
        totalCount: 0,
        selectedIndexes: [],
        isEditMode: false,
        hasMore: true,
        filter: {
            pageNr: 0,
            pageSize: 200,
            persons: [],
            states: [],
            recognitionTypes: []
        }
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

            Vue.set(state, 'totalLoaded', state.totalLoaded + result.items.length)
            state.hasMore = result.totalCount < state.totalLoaded;

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
            state.selectedIndexes = []
        },
        SET_LIST_LOADING: function (state, isloading) {
            state.listLoading = isloading;
        },
        SELECTED: function (state, idx) {
            const current = [...state.selectedIndexes];
            const i = current.indexOf(idx);
            if (i > -1) {
                current.splice(i, 1);
            }
            else {
                current.push(idx);
            }

            Vue.set(state, 'selectedIndexes', current)
        },
        EDIT_MODE_TOGGLE: function (state, value) {
            state.isEditMode = value;
            if (!value) {
                state.selectedIndexes = [];
            }
        },
        ALL_SELECTED: function (state) {
            state.selectedIndexes = [...Array(state.list.length).keys()];
        }
    },
    actions: {
        async search({ commit, state }) {
            try {
                commit('SET_LIST_LOADING', true)

                const res = await searchFaces(state.filter);
                commit("LIST_LOADED", res.data.searchFaces);
            } catch (ex) {
                debugger;
                this.$magic.snack('Error loading', 'ERROR');
            }
        },
        async loadMore({ commit, state, dispatch }) {
            if (state.hasMore) {
                commit('PAGE_NR_INC')
                dispatch('search')
            }
        },
        setPersonFilter({ dispatch, commit }, persons) {
            commit("RESET_FILTER");
            commit("FILTER_PERSONS_SET", persons);
            dispatch('search')
        },
        setRecognitionTypeFilter({ dispatch, commit }, types) {
            commit("RESET_FILTER");
            commit("FILTER_RECOGNITIONTYPE_SET", types);
            dispatch('search')
        },
        setStateFilter({ dispatch, commit }, states) {
            commit("RESET_FILTER");
            commit("FILTER_STATE_SET", states);
            dispatch('search')
        },
        toggleEditMode: function ({ commit }, value) {
            commit("EDIT_MODE_TOGGLE", value)
        },
        select: function ({ commit }, id) {
            commit('SELECTED', id);
        },
        selectAll: function ({ commit }) {
            commit('ALL_SELECTED');
        }
    },
    getters: {
        next: (state, getters, rootState) => step => {
            const currentId = rootState.media.current.id;
            const idx = state.list.findIndex(x => x.media.id == currentId);

            if (idx > -1) {

                const newIndex = idx + step;
                if (currentId !== state.list[newIndex].media.id) {
                    return state.list[newIndex].media.id;
                }
                return null;
            }
        },
    }
}

export default faceModule;
