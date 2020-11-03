const snackbarModule = {
    namespaced: true,
    state: () => ({
        snacks: []
    }),
    mutations: {
        SNACK_ADDED: function (state, snack) {
            state.snacks.push(snack)
        }
    },
    actions: {
        addSnack: function ({ commit }, snack) {
            snack.show = true;
            commit('SNACK_ADDED', snack)
        }
    },
    getters: {}
}

export default snackbarModule;
