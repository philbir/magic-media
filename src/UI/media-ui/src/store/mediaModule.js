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
        currentMediaId: null,
        listLoading: false,
        selectedIndexes: [],
        hasMore: true,
        isEditMode: false,
        filter: {
            pageNr: 0,
            pageSize: 200,
            thumbnailSize: 'M',
            countries: [],
            cities: [],
            persons: [],
        }
    }),
    mutations: {
        MEDIAITEMS_LOADED(state, mediaList) {
            const max = 600;
            const current = [...state.list];
            if (current.length > max) {
                current.splice(0, state.filter.pageSize);
            }

            Vue.set(state, "list", [...current, ...mediaList]);
            state.listLoading = false;
            state.hasMore = mediaList.length > 0
        },
        DETAILS_LOADED(state, media) {
            state.currentMediaId = media.id;
            state.current = Object.assign({}, media);
        },
        FILTER_THUMBNAIL_SIZE_SET(state, size) {
            state.list = [];
            state.filter.pageNr = 0;
            state.filter.thumbnailSize = size;
            state.selectedIndexes = []
        },
        FILTER_PERSONS_SET(state, persons) {
            state.filter.persons = persons;
        },
        FILTER_COUNTRY_SET(state, countries) {
            state.filter.countries = countries;
        },
        FILTER_CITY_SET(state, cities) {
            state.filter.cities = cities;
        },
        PAGE_NR_INC(state) {
            state.filter.pageNr++;
        },
        UPLOAD_DIALOG_TOGGLED: function (state, open) {
            state.uploadDialog.open = open
        },
        SET_MEDIALIST_LOADING: function (state, isloading) {
            state.listLoading = isloading;
        },
        SEARCH_FACETS_LOADED: function (state, facets) {
            Vue.set(state, "facets", facets);
        },
        RESET_FILTER: function (state) {
            state.list = [];
            state.filter.pageNr = 0;
            state.selectedIndexes = []
        },
        MEDIA_CLOSED: function (state) {
            state.currentMediaId = null;
            state.current = null;
        },
        EDIT_MODE_TOGGLE: function (state, value) {
            state.isEditMode = value;
            if (!value) {
                state.selectedIndexes = [];
            }
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
        async loadMore({ commit, state, dispatch }) {
            if (state.hasMore) {
                commit('PAGE_NR_INC')
                dispatch('search')
            }
        },
        async show({ commit }, id) {
            try {
                console.log(id)
                const res = await getById(id);
                commit("DETAILS_LOADED", res.data.mediaById);
            } catch (ex) {
                console.error(ex);
            }
        },
        close({ commit }) {
            commit('MEDIA_CLOSED')
        },
        setThumbnailSize({ dispatch, commit }, size) {
            commit("FILTER_THUMBNAIL_SIZE_SET", size);
            dispatch('search')
        },
        setPersonFilter({ dispatch, commit }, persons) {
            commit("RESET_FILTER");
            commit("FILTER_PERSONS_SET", persons);
            dispatch('search')
        },
        setCountryFilter({ dispatch, commit }, countries) {
            commit("RESET_FILTER");
            commit("FILTER_COUNTRY_SET", countries);
            dispatch('search')
        },
        setCityFilter({ dispatch, commit }, cities) {
            commit("RESET_FILTER");
            commit("FILTER_CITY_SET", cities);
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
                commit('SEARCH_FACETS_LOADED', res.data.facets)
            } catch (ex) {
                console.error(ex);
            }
        },
        toggleUploadDialog: function ({ commit }, open) {
            commit('UPLOAD_DIALOG_TOGGLED', open)
        },
        toggleEditMode: function ({ commit }, value) {
            commit("EDIT_MODE_TOGGLE", value)
        },
        select: function ({ commit }, id) {
            commit('SELECTED', id);
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
