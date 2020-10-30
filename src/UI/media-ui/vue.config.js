module.exports = {
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
