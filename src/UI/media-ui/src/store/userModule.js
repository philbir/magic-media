import Vue from "vue";
import { createInvite, createUserFromPerson, getAllUsers, getMe, saveSharedAlbums, search, getExportProfiles, updateCurrentExportProfile } from "../services/userService";
import { excuteGraphQL } from "./graphqlClient"
import { addSnack } from "./snackService"

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
        exportProfiles: [],
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
        },
        EXPORT_PROFILES_LOADED: function (state, profiles) {
            Vue.set(state, "exportProfiles", [...profiles])
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
            }
        },
        resetError: function ({ commit }) {
            commit("ERROR_RESET");
        },
        async createFromPerson({ commit, dispatch }, payload) {
            const result = await excuteGraphQL(() => createUserFromPerson(payload.personId, payload.email), dispatch);

            if (result.success) {
                commit("USER_ADDED", result.data.User_CreateFromPerson.user);

                addSnack(dispatch, `User created ${result.data.User_CreateFromPerson.user.email}`)

            }
        },
        async createInvite({ dispatch }, id) {
            const result = await excuteGraphQL(() => createInvite(id), dispatch);

            if (result.success) {
                //commit("INVITE_CREATED", result.data.User_CreateInvite.user);
                addSnack(dispatch, `Invite created for ${result.data.User_CreateInvite.user.name}`)
            }
        },
        async saveSharedAlbums({ dispatch }, input) {
            const result = await excuteGraphQL(() => saveSharedAlbums(input), dispatch);

            if (result.success) {
                addSnack(dispatch, `Shared albums saved.`)
            }
        },
        async getExportProfiles({ dispatch, commit }) {
            const result = await excuteGraphQL(() => getExportProfiles(), dispatch);

            commit("EXPORT_PROFILES_LOADED", result.data.mediaExportProfiles);
        },
        async updateCurrentExportProfile({ dispatch }, profile) {
            const result = await excuteGraphQL(() => updateCurrentExportProfile({ profileId: profile.id }), dispatch);
            if (result.success) {
                addSnack(dispatch, `Profile: '${profile.name}' set.`)
            }
            dispatch("getExportProfiles")
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
                    destroy: getters["hasPermission"]('MEDIA_DESTROY'),
                    download: getters["hasPermission"]('MEDIA_DOWNLOAD'),
                },
                face: {
                    edit: getters["hasPermission"]('FACE_EDIT'),
                },
                person: {
                    edit: getters["hasPermission"]('PERSON_EDIT'),
                    delete: getters["hasPermission"]('PERSON_DELETE'),
                },
                album: {
                    edit: getters["hasPermission"]('ALBUM_EDIT'),
                    delete: getters["hasPermission"]('ALBUM_DELETE'),
                },
                user: {
                    edit: getters["hasPermission"]('USER_EDIT'),
                },
            }
        }
    }
};

export default userModule;
