import Vue from "vue";
import Vuex from "vuex";

import router from "../router";
import albumModule from "./albumModule";
import faceModule from "./faceModule";
import mapModule from "./mapModule";
import mediaModule from "./mediaModule";
import personModule from "./personModule";
import snackbarModule from "./snackbarModule";
import userModule from "./userModule";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    navDrawerOpen: false
  },
  mutations: {
    NAV_DRAWER_OPEN(state, isOpen) {
      state.navDrawerOpen = isOpen;
    }
  },
  actions: {
    setMobile: function ({ commit }, isMobile) {
      commit('media/MOBILE_DETECTED', isMobile, { root: true })
    },
    openNavDrawer: function ({ commit }, open) {
      commit('NAV_DRAWER_OPEN', open)
    }
  },
  getters: {
    next: (state, getters, rootState, rootGetters) => step => {
      if (router.currentRoute.name === "Faces") {
        return rootGetters["face/next"](step);
      } else {
        return rootGetters["media/next"](step);
      }
    }
  },
  modules: {
    snackbar: snackbarModule,
    media: mediaModule,
    face: faceModule,
    person: personModule,
    album: albumModule,
    map: mapModule,
    user: userModule
  }
});
