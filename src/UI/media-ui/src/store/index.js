import Vue from "vue";
import Vuex from "vuex";

import mediaModule from "./mediaModule";
import faceModule from "./faceModule";
import personModule from "./personModule";
import snackbarModule from "./snackbarModule";
import router from '../router'

/* eslint-disable no-debugger */

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
  },
  mutations: {

  },
  actions: {

  },
  getters: {
    next: (state, getters, rootState, rootGetters) => step => {
      if (router.currentRoute.name === 'Faces') {
        return rootGetters['face/next'](step);
      }
      else {
        return rootGetters['media/next'](step);

      }
    }
  },
  modules: {
    snackbar: snackbarModule,
    media: mediaModule,
    face: faceModule,
    person: personModule
  }
});
