import * as signalR from '@microsoft/signalr';

export const createConnection = (url:string) => {
    return new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl(url,{
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();
}


export const startConenection = async (connection: signalR.HubConnection) =>{
    try{
        await connection.start();
        console.log("Connected to the hub");
    }
    catch(err){
        console.log("Error while connecting to the hub "+err);
    }
}


export const stopConnection = async (connection: signalR.HubConnection) =>{
    try{
        await connection.stop();
        console.log("Disconnected from the hub");
    }catch(err){
        console.log("Error while disconnecting from the hub "+err);
    }
}

export const invokeServerMethod = async (
    connection: signalR.HubConnection,
    methodName: string,
    ...args: unknown[]
) => {
    try {
        await connection.invoke(methodName, ...args);
    } catch (err) {
        console.error(`Error invoking ${methodName}:`, err);
    }
};

export const onServerMethod = async(
    connection: signalR.HubConnection,
    methodName: string,
    callback: (...args: unknown[]) => void
) =>{
    connection.on(methodName,callback);
}

export const offServerMethod = async(
    connection: signalR.HubConnection,
    methodName: string,
    callback: (...args: unknown[]) => void
) =>{
    connection.off(methodName,callback);
}