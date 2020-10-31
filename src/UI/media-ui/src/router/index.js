import Vue from 'vue'
import VueRouter from 'vue-router'
//import Home from '../views/Home.vue'
import MediaList from '../views/MediaList.vue'
import Media from '../views/Media.vue'
import MediaCarousel from '../views/MediaCarousel.vue'
import Upload from '../views/Upload.vue'

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
  {
    path: '/mediac/:id',
    name: 'mediac',
    component: MediaCarousel
  },
  {
    path: '/upload',
    name: 'upload',
    component: Upload
  },
]

const router = new VueRouter({
  mode: 'history',
  routes
})

export default router
