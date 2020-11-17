import Vue from "vue";
import VueRouter from "vue-router";

import AlbumFilter from "../components/Album/AlbumFilter";
import AlbumList from "../components/Album/AlbumList";
import DefaultAppBar from "../components/DefaultAppBar";
import FaceAppBar from "../components/FaceAppBar";
import FaceFilter from "../components/FaceFilter";
import MapView from "../components/Map/MapView";
import MediaAppBar from "../components/MediaAppBar";
import MediaFilter from "../components/MediaFilter";
import PersonFilter from "../components/PersonFilter";
import FaceList from "../views/FaceList.vue";
import MediaList from "../views/MediaList.vue";
import PersonList from "../views/PersonList";

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
  {
    path: "/albums",
    name: "Albums",
    components: {
      default: AlbumList,
      left: AlbumFilter,
      appbar: DefaultAppBar
    }
  },
  {
    path: "/map",
    name: "Map",
    components: {
      default: MapView,
      left: null,
      appbar: DefaultAppBar
    }
  }
];

const router = new VueRouter({
  mode: "history",
  routes
});

export default router;
