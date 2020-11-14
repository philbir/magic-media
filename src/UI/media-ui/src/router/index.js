import Vue from "vue";
import VueRouter from "vue-router";
import MediaList from "../views/MediaList.vue";
import FaceList from "../views/FaceList.vue";
import PersonList from "../views/PersonList";
import MediaFilter from "../components/MediaFilter"
import MediaAppBar from "../components/MediaAppBar"
import FaceAppBar from "../components/FaceAppBar"
import DefaultAppBar from "../components/DefaultAppBar"
import FaceFilter from "../components/FaceFilter"
import PersonFilter from "../components/PersonFilter"

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    components: {
      default: MediaList,
      left: MediaFilter,
      appbar: MediaAppBar
    }
  },
  {
    path: "/faces",
    name: "Faces",
    components: {
      default: FaceList,
      left: FaceFilter,
      appbar: FaceAppBar
    }
  },
  {
    path: "/persons",
    name: "Persons",
    components: {
      default: PersonList,
      left: PersonFilter,
      appbar: DefaultAppBar
    }
  },
];

const router = new VueRouter({
  mode: "history",
  routes
});

export default router;
