import Vue from 'vue'
import VueRouter from 'vue-router'
//import Home from '../views/Home.vue'
import MediaList from '../views/MediaList.vue'
import Media from '../views/Media.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: MediaList
  },
  {
    path: '/media/:id',
    name: 'media',
    component: Media
  },
]

const router = new VueRouter({
  mode: 'history',
  routes
})

export default router
