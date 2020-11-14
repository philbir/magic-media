import { addItems } from "../services/albumService";

const albumModule = {
  namespaced: true,
  state: () => ({
    albums: []
  }),
  mutations: {
    ITEM_ADDED(state, album) {
      console.log(state, album);
    }
  },
  actions: {
    async addItems({ commit }, input) {
      try {
        const res = await addItems(input);
        commit("ITEM_ADDED", res.data.addItemsToAlbum.album);
      } catch (ex) {
        console.error(ex);
      }
    }
  },
  getters: {}
};

export default albumModule;
