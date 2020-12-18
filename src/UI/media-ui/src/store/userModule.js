
import { createUserFromPerson, getAllUsers, getMe } from "../services/userService";

const userModule = {
    namespaced: true,
    state: () => ({
        all: [],
        me: {}
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
        async getAll({ commit }) {
            try {
                const res = await getAllUsers();
                commit("ALL_USERS_LOADED", res.data.allUsers);
            } catch (ex) {
                console.error(ex);
            }
        },
        async getMe({ commit }) {
            try {
                const res = await getMe();
                commit("ME_LOADED", res.data.me);
            } catch (ex) {
                console.error(ex);
            }
        },
        async createFromPerson({ commit }, payload) {
            try {
                const res = await createUserFromPerson(payload.personId, payload.email);
                commit("USER_ADDED", res.data.User_CreateFromPerson.user);
            } catch (ex) {
                console.error(ex);
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
