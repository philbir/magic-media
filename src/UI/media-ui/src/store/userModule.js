import Vue from "vue";
import { createInvite, createUserFromPerson, getAllUsers, getMe, search } from "../services/userService";
import { excuteGraphQL } from "./graphqlClient"

const userModule = {
    namespaced: true,
    state: () => ({
        all: [],
        me: null,
        hasMore: true,
        listLoading: false,
        error: false,
        totalCount: 0,
        list: [],
        filter: {
            pageNr: 0,
            pageSize: 50,
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
        FILTER_SET(state, filter) {
            state.filter = Object.assign(state.filter, filter);
        },
        USER_ADDED: function (state, user) {
            state.all.push(user);
        },
        ALL_USERS_LOADED: function (state, users) {
            state.all = users;
        },
        ME_LOADED: function (state, user) {
            state.me = user;
        },
        ERROR: function (state) {
            state.error = true;
        },
        ERROR_RESET: function (state) {
            state.error = false;
        }
    },
    actions: {
        async search({ commit, state, dispatch }) {
            commit("SET_SEARCH_LOADING", true);
            const result = await excuteGraphQL(() => search(state.filter), dispatch);

            if (result.success) {
                commit("SEARCH_COMPLETED", result.data.searchUsers);
            }

            commit("SET_SEARCH_LOADING", false);
        },
        filter: function ({ commit, dispatch }, filter) {
            commit("FILTER_SET", filter)
            dispatch('search');
        },
        async getAll({ commit, dispatch }) {
            const result = await excuteGraphQL(() => getAllUsers(), dispatch);

            if (result.success) {
                commit("ALL_USERS_LOADED", result.data.allUsers);
            }
        },
        async getMe({ commit, dispatch }) {
            const result = await excuteGraphQL(() => getMe(), dispatch);

            if (result.success) {
                commit("ME_LOADED", result.data.me);
            }
            else {
                commit("ERROR");
                console.log('Load ME Error')
            }
        },
        resetError: function ({ commit }) {
            commit("ERROR_RESET");
        },
        async createFromPerson({ commit, dispatch }, payload) {
            const result = await excuteGraphQL(() => createUserFromPerson(payload.personId, payload.email), dispatch);

            if (result.success) {
                commit("USER_ADDED", result.data.User_CreateFromPerson.user);
            }
        },
        async createInvite({ dispatch }, id) {
            const result = await excuteGraphQL(() => createInvite(id), dispatch);

            if (result.success) {
                //commit("INVITE_CREATED", result.data.User_CreateInvite.user);
                dispatch(
                    "snackbar/addSnack",
                    { text: `Invite created for ${result.data.User_CreateInvite.user.name}`, type: "SUCCESS" },
                    { root: true }
                );
            }
        },
    },
    getters: {
        hasPermission: state => permission => {
            if (state.me && state.me.permissions) {
                return state.me.permissions.includes(permission)
            }
            return false;
        },
        userActions: (state, getters) => {
            return {
                media: {
                    edit: getters["hasPermission"]('MEDIA_EDIT'),
                    upload: getters["hasPermission"]('MEDIA_UPLOAD'),
                },
                face: {
                    edit: getters["hasPermission"]('FACE_EDIT'),
                },
                person: {
                    edit: getters["hasPermission"]('PERSON_EDIT'),
                },
                album: {
                    edit: getters["hasPermission"]('ALBUM_EDIT'),
                },
            }
        }
    }
};

export default userModule;
