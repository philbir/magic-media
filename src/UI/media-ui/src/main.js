import * as GmapVue from "gmap-vue";
import { DateTime } from "luxon";
import VueMask from "v-mask";
import Vue from "vue";
import VueCoreVideoPlayer from "vue-core-video-player";

import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import router from "./router";
import signalrHub from "./signalrHub";
import store from "./store";
import './registerServiceWorker'

Vue.config.productionTip = false;

const magicPlugin = {
  install: Vue => {
    Vue.prototype.$magic = {
      self: this,
      snack: function (text, type = "INFO") {
        store.dispatch("snackbar/addSnack", {
          text,
          type
        });
      }
    };
  }
};

Vue.use(GmapVue, {
  load: {
    key: process.env.VUE_APP_GOOGLE_MAPS_KEY,
    libraries: "places"
  },

  installComponents: true
});

Vue.use(VueCoreVideoPlayer);
Vue.use(magicPlugin);
Vue.use(signalrHub);
Vue.use(VueMask);

Vue.use(GmapVue, {
  load: {
    key: process.env.VUE_APP_GOOGLE_MAPS_KEY,
    libraries: "places"
  },

  installComponents: true
});

Vue.filter("dateformat", function (value, format = "DATE_SHORT") {
  if (!value) return "";

  var date = DateTime.fromISO(value);
  if (DateTime.isDateTime(date)) {
    return date.toLocaleString(DateTime[format]);
  }
  return "";
});

Vue.directive('longpress', {
  bind: function (el, binding) {

    let pressTimer = null

    let start = (e) => {

      if (e.type === 'click' && e.button !== 0) {
        return;
      }

      if (pressTimer === null) {
        pressTimer = setTimeout(() => {
          handler()
        }, 1000)
      }
    }

    let cancel = () => {
      if (pressTimer !== null) {
        clearTimeout(pressTimer)
        pressTimer = null
      }
    }
    const handler = (e) => {
      binding.value(e)
    }

    el.addEventListener("mousedown", start);
    el.addEventListener("touchstart", start);

    el.addEventListener("click", cancel);
    el.addEventListener("mouseout", cancel);
    el.addEventListener("touchend", cancel);
    el.addEventListener("touchcancel", cancel);
  }
})

Vue.filter("toISODate", function (value) {
  if (!value) return "";

  var date = DateTime.fromISO(value);
  if (DateTime.isDateTime(date)) {
    return date.toISODate();
  }
  return "";
});

new Vue({
  vuetify,
  router,
  store,
  render: h => h(App)
}).$mount("#app");
