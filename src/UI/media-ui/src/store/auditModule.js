import Vue from "vue";
import { search } from "../services/auditService";
import { excuteGraphQL } from "./graphqlClient"

const auditModule = {
    namespaced: true,
    state: () => ({
        hasMore: true,
        listLoading: false,
        totalCount: 0,
        list: [],
        filter: {
            pageNr: 0,
            pageSize: 10,
            userId: null,
            success: null
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
        FILTER_SET(state, filter) {
            state.filter = Object.assign(state.filter, filter);
        },
        PAGING_SET(state, paging) {
            state.filter.pageNr = paging.pageNr;
            state.filter.pageSize = paging.pageSize;
        }
    },
    actions: {
        async search({ commit, state, dispatch }) {
            commit("SET_SEARCH_LOADING", true);
            console.log(state.filter)
            const result = await excuteGraphQL(() => search(state.filter), dispatch);

            if (result.success) {
                commit("SEARCH_COMPLETED", result.data.searchAuditEvents);
            }

            commit("SET_SEARCH_LOADING", false);
        },
        filter: function ({ commit, dispatch }, filter) {
            commit("FILTER_SET", filter)
            dispatch('search');
        },
        setPaging: function ({ commit, dispatch }, paging) {
            commit("PAGING_SET", paging);
            dispatch('search');
        }

    },
    getters: {

    }
};

export default auditModule;
