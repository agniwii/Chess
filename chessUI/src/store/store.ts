import { configureStore } from "@reduxjs/toolkit";
import chessReducer from "../features/chess/chessSlice";

const store = configureStore({
    reducer:{
        chess: chessReducer
    },
    middleware:(getDefaultMiddleware) => 
        getDefaultMiddleware({
            serializableCheck:{
                ignoredActions: ['chess/startSignalRConnection'],
                ignoredActionPaths: ['payload.connection']
            
            }
        })
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export default store;