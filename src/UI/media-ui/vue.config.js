

module.exports = {
  configureWebpack: {
    devtool: 'source-map',
  },
  pluginOptions: {
    apollo: {
      lintGQL: false
    }
  },
  pwa: {
    name: 'Magic Media',
    themeColor: '#1a237e',
    msTileColor: '#FFFFFF',
    appleMobileWebAppCapable: 'yes',
    appleMobileWebAppStatusBarStyle: 'black',

    workboxPluginMode: 'InjectManifest',
    workboxOptions: {
      swSrc: 'src/sw.js',
    }
  },
  transpileDependencies: ["vuetify"],
  devServer: {
    host: "0.0.0.0",
    proxy: {
      "/api": {
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
          "x-csrf": 1
        }
      },
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
          "x-csrf": 1
        }
      },
      "/bff": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "x-csrf": 1
        }
      },
      "/signalr": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
          "x-csrf": 1
        }
      }
    }
  }
};
