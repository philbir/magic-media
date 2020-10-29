module.exports = {

  "transpileDependencies": [
    "vuetify"
  ],
  devServer: {
    proxy: {
      "/api": {
        changeOrigin: true,
        target: "http://localhost:5000/"
      },
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: "http://localhost:5000/"
      }
    }
  }
}