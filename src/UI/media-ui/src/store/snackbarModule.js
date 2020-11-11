const snackbarModule = {
    namespaced: true,
    state: () => ({
        snacks: [],
        notifications: [],
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
        },
        addNotification: function ({ commit }, notification) {
            commit('NOTIFICATION_ADDED', notification)
        },
        operationStarted: function ({ commit }, operation) {
            console.log(operation)
        }
    },
    getters: {}
}

export default snackbarModule;
