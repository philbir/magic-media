import Vue from 'vue'
import Vuex from 'vuex'
import { searchMedia, getById } from '../services/mediaService'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    mediaList: [],
    currentMedia: null
  },
  mutations: {
    setMediaList(state, mediaList){
      state.mediaList.push(... mediaList);
    },
    setSelectedMedia(state, media){
      console.log('MUT', media)
      state.currentMedia = Object.assign({}, media);
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
        const res = await getById(id);
        commit('setSelectedMedia', res.data.mediaById);
      }
      catch (ex){
        console.error(ex)
      }
    }
  },
  modules: {
  }
})
