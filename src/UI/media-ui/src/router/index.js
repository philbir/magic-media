import Vue from "vue";
import VueRouter from "vue-router";
import MediaList from "../views/MediaList.vue";

//import Home from '../views/Home.vue'

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: MediaList
  },
];

const router = new VueRouter({
  mode: "history",
  routes
});

export default router;
