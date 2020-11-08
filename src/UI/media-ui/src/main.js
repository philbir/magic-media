import Vue from "vue";

import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import router from "./router";
import store from "./store";
import { DateTime } from 'luxon'
import Fragment from 'vue-fragment'


Vue.config.productionTip = false;

const magicPlugin = {

  install: (Vue) => {
    Vue.prototype.$magic = {
      self: this,
      snack: function (text, type = 'INFO') {
        store.dispatch("snackbar/addSnack", {
          text,
          type
        });
      }
    };
  }
}

Vue.use(magicPlugin)
Vue.use(Fragment.Plugin)

Vue.filter('dateformat', function (value, format = 'DATE_SHORT') {
  if (!value)
    return '';

  var date = DateTime.fromISO(value);
  if (DateTime.isDateTime(date)) {
    return date.toLocaleString(DateTime[format])
  }
  return '';

})

new Vue({
  vuetify,
  router,
  store,
  render: h => h(App)
}).$mount("#app");
