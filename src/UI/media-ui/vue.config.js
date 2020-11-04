module.exports = {
<<<<<<< HEAD
=======
  pluginOptions: {
    apollo: {
        lintGQL: false
    }
},
>>>>>>> d6e340bd94b7733ad7fd351f7cbd24f5251d7bf9
  transpileDependencies: ["vuetify"],
  devServer: {
    proxy: {
      "/api": {
        changeOrigin: true,
        target: "https://magic-media-demo.birbaum.me/"
      },
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: "https://magic-media-demo.birbaum.me/"
      }
    }
  }
};
