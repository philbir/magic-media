import { getGeoLocationClusters } from "../services/mediaService";

const mapModule = {
    namespaced: true,
    state: () => ({
        clusters: [],
        place: null,
        loading: false
    }),
    mutations: {
        CLUSTERS_LOADED(state, clusters) {
            state.clusters = [...clusters];
            state.loading = false;
        },
        PLACE_SET(state, place) {
            state.place = place;
        },
        SET_LOADING(state, loading) {
            state.loading = loading;
        }
    },
    actions: {
        async getClusters({ commit }, input) {
            try {
                commit("SET_LOADING", true);
                const res = await getGeoLocationClusters(input);
                commit("CLUSTERS_LOADED", res.data.geoLocationClusters);

            } catch (ex) {
                console.error(ex);
            }
        },
        setPlace: function ({ commit }, place) {
            commit('PLACE_SET', place)
        }
    },
    getters: {}
};

export default mapModule;
