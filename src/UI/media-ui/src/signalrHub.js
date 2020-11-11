import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'
export default {
    install(Vue) {
        const connection = new HubConnectionBuilder()
            .withUrl('/signalr')
            .configureLogging(LogLevel.Information)
            .build()

        connection.start()
        Vue.prototype.$socket = connection;
    }
}