import * as GmapVue from "gmap-vue";
import { DateTime } from "luxon";
import VueMask from "v-mask";
import Vue from "vue";

import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import router from "./router";
import signalrHub from "./signalrHub";
import store from "./store";

Vue.config.productionTip = false;

const magicPlugin = {
  install: Vue => {
    Vue.prototype.$magic = {
      self: this,
      snack: function(text, type = "INFO") {
        store.dispatch("snackbar/addSnack", {
          text,
          type
        });
      }
    };
  }
};

Vue.use(magicPlugin);
Vue.use(signalrHub);
Vue.use(VueMask);
Vue.use(GmapVue, {
  load: {
    key: "AIzaSyDScX2p_g7Qqj1pFzQkA999SevEOi6u1c8",
    libraries: "places" // This is required if you use the Autocomplete plugin
  },

  installComponents: true
});

Vue.filter("dateformat", function(value, format = "DATE_SHORT") {
  if (!value) return "";

  var date = DateTime.fromISO(value);
  if (DateTime.isDateTime(date)) {
    return date.toLocaleString(DateTime[format]);
  }
  return "";
});

new Vue({
  vuetify,
  router,
  store,
  render: h => h(App)
}).$mount("#app");
