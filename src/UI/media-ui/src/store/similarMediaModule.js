
import {
    getSimilarGroups
} from "../services/mediaService";

import { excuteGraphQL } from "./graphqlClient"
//import { addSnack } from "./snackService"

const similarMediaModule = {
    namespaced: true,
    state: () => ({
        loading: false,
        groups: [],
        filter: {
            pageNr: 0,
            pageSize: 50,
            hashType: "IMAGE_PERCEPTUAL_HASH",
            similarity: 99.9
        },
        selectedGroupId: null
    }),
    mutations: {
        SET_LOADING(state, loading) {
            state.loading = loading;
        },
        GROUPS_LOADED(state, groups) {
            state.loading = false
            state.groups = groups;
        },
        FILTER_SET(state, filter) {
            state.filter = Object.assign(state.filter, filter, { pageNr: 0 })
        },
        PAGENR_SET(state, pageNr) {
            state.filter.pageNr = pageNr;
        },
        GROUP_SELECTED(state, group) {
            state.selectedGroupId = group.id
        }
    },
    actions: {
        async getGroups({ state, commit, dispatch }) {
            commit("SET_LOADING", true);
            const result = await excuteGraphQL(() => getSimilarGroups(state.filter), dispatch);
            if (result.success) {

                commit("GROUPS_LOADED", result.data.similarMediaGroups);
            }
            else {
                commit("SET_LOADING", false);
            }
        },
        async setFilter({ commit, dispatch }, filter) {
            commit('FILTER_SET', filter);
            dispatch('getGroups');
        },
        async setPage({ state, commit, dispatch }, value) {
            commit('PAGENR_SET',
                state.filter.pageNr + value
            );
            dispatch('getGroups');
        },
        selectGroup: function ({ commit }, group) {
            commit("GROUP_SELECTED", group)
        }
    },
    getters: {

    }
};

export default similarMediaModule;
