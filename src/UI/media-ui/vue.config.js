module.exports = {
  pluginOptions: {
    apollo: {
      lintGQL: false
    }
  },
  transpileDependencies: ["vuetify"],
  devServer: {
    proxy: {
      "/api": {
        changeOrigin: true,
        target: "https://magic-media-preview.birbaum.me/"
      },
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: "https://magic-media-preview.birbaum.me/"
      }
    }
  }
};
