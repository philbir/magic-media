import Vue from "vue";
import VueRouter from "vue-router";

import Media from "../views/Media.vue";
import MediaCarousel from "../views/MediaCarousel.vue";
import MediaList from "../views/MediaList.vue";

//import Home from '../views/Home.vue'

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: MediaList
  },
  {
    path: "/media/:id",
    name: "media",
    component: Media,
    meta: { transition: "face", fullscreen: true },
    
  },
  {
    path: "/mediac/:id",
    name: "mediac",
    component: MediaCarousel
  }
];

const router = new VueRouter({
  mode: "history",
  routes
});

export default router;
