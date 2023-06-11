import Vue from "vue";
import { excuteGraphQL } from "./graphqlClient"
import { changeMatte, deleteMedia, getDevices, getFeatures, getMedias, selectMedia, setFilter } from "../services/samsungTvService";

const samsungTvModule = {
    namespaced: true,
    state: () => ({
        selectedDevice: null,
        devices: {
            loading: false,
            list: [],
        },
        media: {
            loading: false,
            list: [],
        },
        features: {
            filters: [],
            mattes: [],
        }
    }),
    mutations: {
        SET_DEVICES_LOADING(state, loading) {
            state.devices.loading = loading;
        },
        DEVICES_LOADED(state, devices) {
            Vue.set(state.devices, "list", [...devices]);
            state.devices.loading = false;
        },
        FEATURES_LOADED(state, features) {
            state.features = features;
        },
        SET_MEDIA_LOADING(state, loading) {
            state.media.loading = loading;
        },
        MEDIAS_LOADED(state, medias) {
            Vue.set(state.media, "list", [...medias]);
            state.media.loading = false;
        },
        DEVICE_SELECTED(state, device) {
            state.selectedDevice = state.devices.list.find(x => x.name === device);
        },
        MEDIA_SELECTED(state, id) {

            for (let i = 0; i < state.media.list.length; i++) {
                if (state.media.list[i].id === id) {
                    state.media.list[i].selected = true;
                }
                else {
                    state.media.list[i].selected = false;
                }
            }
        },
        MEDIA_DELETED(state, id) {
            var index = state.media.list.findIndex(x => x.id === id);
            if (index > -1) {
                state.media.list.splice(index, 1);
            }
        },
        MATTE_CHANGED(state, data) {
            var index = state.media.list.findIndex(x => x.id === data.id);
            if (index > -1) {
                state.media.list[index].matte_id = data.matte;
            }
        },
    },
    actions: {
        async getDevices({ commit, dispatch }) {
            commit("SET_DEVICES_LOADING", true);
            const result = await excuteGraphQL(() => getDevices(), dispatch);

            if (result.success) {
                commit("DEVICES_LOADED", result.data.samsungTvDevices);

                var onlineDevices = result.data.samsungTvDevices.filter(x => x.online);
                if (onlineDevices.length > 0) {
                    dispatch("getMedias", onlineDevices[0].name);
                }
            }
        },
        async getMedias({ commit, dispatch }, device) {
            commit("SET_MEDIA_LOADING", true);
            commit("DEVICE_SELECTED", device);
            const result = await excuteGraphQL(() => getMedias(device), dispatch);

            if (result.success) {
                commit("MEDIAS_LOADED", result.data.samsungTvMedia);
            }
        },
        async getFeatures({ state, commit, dispatch }) {

            const result = await excuteGraphQL(() => getFeatures(state.selectedDevice.name), dispatch);

            if (result.success) {
                commit("FEATURES_LOADED", result.data.samsungTvFeatures);
            }
        },
        async selectMedia({ commit, dispatch, state }, id) {
            const result = await excuteGraphQL(() => selectMedia({ device: state.selectedDevice.name, id }), dispatch);

            if (result.success) {
                commit("MEDIA_SELECTED", id);
            }
        },
        async deleteMedia({ commit, dispatch, state }, id) {
            const result = await excuteGraphQL(() => deleteMedia({ device: state.selectedDevice.name, id }), dispatch);
            if (result.success) {
                commit("MEDIA_DELETED", id);
            }
        },
        async changeMatte({ commit, dispatch, state }, payload) {
            const result = await excuteGraphQL(() =>
                changeMatte({ device: state.selectedDevice.name, id: payload.id, matte: payload.matte }), dispatch);
            if (result.success) {
                commit("MATTE_CHANGED", payload.id);
            }
        },
        async setFilter({ dispatch, state }, id, payload) {
            const result = await excuteGraphQL(() =>
                setFilter({ device: state.selectedDevice.name, id: payload.id, filter: payload.filter }), dispatch);
            console.log(result);
        },
    },
    getters: {

    }
};

export default samsungTvModule;