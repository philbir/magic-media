
import { createUserFromPerson, getAllUsers, getMe } from "../services/userService";
import { excuteGraphQL } from "./graphqlClient"

const userModule = {
    namespaced: true,
    state: () => ({
        all: [],
        me: null
    }),
    mutations: {
        USER_ADDED: function (state, user) {
            state.all.push(user);
        },
        ALL_USERS_LOADED: function (state, users) {
            state.all = users;
        },
        ME_LOADED: function (state, user) {
            state.me = user;
        }
    },
    actions: {
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
        },
        async createFromPerson({ commit, dispatch }, payload) {
            const result = await excuteGraphQL(() => createUserFromPerson(payload.personId, payload.email), dispatch);

            if (result.success) {
                commit("USER_ADDED", result.data.User_CreateFromPerson.user);
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
            console.log(state)
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
