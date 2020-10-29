import Vue from 'vue'
import Vuex from 'vuex'
import { searchMedia, getById } from '../services/mediaService'
import { getAllPersons } from '../services/personService'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    mediaList: [],
    currentMedia: null,
    persons: []
  },
  mutations: {
    setMediaList(state, mediaList){
      Vue.set(state, 'mediaList', [... mediaList])
    },
    setSelectedMedia(state, media){
      state.currentMedia = Object.assign({}, media);
    },
    PERSONS_LOADED(state, persons){
      Vue.set(state, 'persons', [... persons])
    }
  },
  actions: {
    async searchMedia ({commit}) {
      try
      {
        const res = await searchMedia();
        commit('setMediaList', res.data.searchMedia);
      }
      catch (ex){
        console.error(ex)
      }
    },
    async loadMediaDetails ({commit}, id) {
      try
      {
        const res =  await getById(id);
        commit('setSelectedMedia', res.data.mediaById);
      }
      catch (ex){
        console.error(ex)
      }
    },
    async getAllPersons ({commit}) {
      try
      {
        const res = await getAllPersons();
        console.log(res.data)
        commit('PERSONS_LOADED', res.data.persons);
      }
      catch (ex){
        console.error(ex)
      }
    }
  },
  modules: {
  }
})
