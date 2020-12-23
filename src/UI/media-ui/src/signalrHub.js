import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
export default {
    install(Vue) {
        const connection = new HubConnectionBuilder()
            .withUrl('/signalr')
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build()

        Vue.prototype.$socket = connection;
    }
}