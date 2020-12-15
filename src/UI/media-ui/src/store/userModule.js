
import { createUserFromPerson, getAllUsers } from "../services/userService";

const userModule = {
    namespaced: true,
    state: () => ({
        all: []
    }),
    mutations: {
        USER_ADDED: function (state, user) {
            state.all.push(user);
        },
        ALL_USERS_LOADED: function (state, users) {
            state.all = users;
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

    }
};

export default userModule;
