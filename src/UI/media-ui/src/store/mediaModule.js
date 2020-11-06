import Vue from 'vue';

import { getById, searchMedia, getSearchFacets } from "../services/mediaService";

const mediaModule = {
    namespaced: true,
    state: () => ({
        uploadDialog: {
            open: false
        },
        list: [],
        facets: null,
        current: null,
        listLoading: false,
        filter: {
            pageSize: 100,
            thumbnailSize: 'M'
        }
    }),
    mutations: {
        MEDIAITEMS_LOADED(state, mediaList) {
            Vue.set(state, "list", [...mediaList]);
            state.listLoading = false;
        },
        DETAILS_LOADED(state, media) {
            state.current = Object.assign({}, media);
        },
        FILTER_THUMBNAIL_SIZE_SET(state, size) {
            state.filter.thumbnailSize = size;
        },
        UPLOAD_DIALOG_TOGGLED: function (state, open) {
            state.uploadDialog.open = open
        },
        SET_MEDIALIST_LOADING: function (state, isloading) {
            state.listLoading = isloading;
        },
        SEARCH_FACETS_LOADED: function (state, facets) {
            Vue.set(state, "facets", facets);
        }
    },
    actions: {
        async search({ commit, state }) {
            try {
                commit('SET_MEDIALIST_LOADING', true)

                const res = await searchMedia(state.filter);
                commit("MEDIAITEMS_LOADED", res.data.searchMedia);
            } catch (ex) {
                this.$magic.snack('Error loading', 'ERROR');
            }
        },
        setThumbnailSize({ dispatch, commit }, size) {
            commit("FILTER_THUMBNAIL_SIZE_SET", size);
            dispatch('search')
        },
        async loadDetails({ commit }, id) {
            try {
                const res = await getById(id);
                commit("DETAILS_LOADED", res.data.mediaById);
            } catch (ex) {
                console.error(ex);
            }
        },
        async getSearchFacets({ commit }) {
            try {
                const res = await getSearchFacets();
                console.log(res)
                commit('SEARCH_FACETS_LOADED', res.data.facets)
            } catch (ex) {
                console.error(ex);
            }
        },
        toggleUploadDialog: function ({ commit }, open) {
            commit('UPLOAD_DIALOG_TOGGLED', open)
        }
    },
    getters: {
        next: state => step => {
            const currentId = state.current.id;
            const idx = state.list.findIndex(x => x.id == currentId);
            if (idx > -1) {
                const newIndex = idx + step;
                if (newIndex > state.list.length) return null;
                return state.list[newIndex];
            }

            return null;
        }
    },
}

export default mediaModule;
