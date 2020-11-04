import Vue from "vue";
import Vuex from "vuex";


import mediaModule from "./mediaModule";
import personModule from "./personModule";
import snackbarModule from "./snackbarModule";

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

  },
  modules: {
    snackbar: snackbarModule,
    media: mediaModule,
    person: personModule
  }
});
