module.exports = {
  configureWebpack: {
    devtool: 'source-map'
  },
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
        target: process.env.API_BASE_URL
      },
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL
      },
      "/signalr": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL
      }
    }
  }
};
