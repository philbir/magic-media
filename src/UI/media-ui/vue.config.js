
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
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
        }
      },
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
        }
      },
      "/signalr": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
        }
      }
    }
  }
};
