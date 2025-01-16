import { useEffect,useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { MoveRequest, PossibleMovesResponse } from '../../models';
import { createConnection, invokeServerMethod, onServerMethod } from '../../utilities';
import { setSelectedPosition, setPossibleMoves, movePiece } from '../../features';
import { RootState } from '../../store/store';

export const useChessGame = (gameId: string) => {
    const dispatch = useDispatch();
    const { board, selectedPosition, possibleMoves } = useSelector((state: RootState) => state.chess);
    const [connection,setConnection] = useState<signalR.HubConnection | null>(null);

    useEffect(() => {
        const connection = createConnection("http://localhost:5042/chesshub");
        setConnection(connection);
        onServerMethod(connection, 'ReceivePossibleMoves', (...args: unknown[]) => {
            const response = args[0] as PossibleMovesResponse;
            dispatch(setPossibleMoves(response.possibleMoves));
        });

        onServerMethod(connection, 'ReceiveMove', (result: any) => {
            if (result.isValidMove) {
                dispatch(movePiece({ from: result.from, to: result.to }));
                console.log('Move successful:', result.message);
            } else {
                console.error('Invalid move:', result.message);
            }
        });
    }, [dispatch]);

    const handleSquareClick = async (x: number, y: number) => {
        if (selectedPosition) {
            const moveRequest: MoveRequest = {
                gameId,
                from: selectedPosition,
                playerId: connection?.connectionId || '',
                to: { x, y },
            };
            if (connection) {
                await invokeServerMethod(connection, 'MovePiece', moveRequest);
            }
            dispatch(setSelectedPosition(null));
            if (connection) {
                if (connection) {
                    await invokeServerMethod(connection, 'GetPossibleMoves', gameId, x, y);
                }
            }
        } else {
            if (connection) {
                await invokeServerMethod(connection, 'GetPossibleMoves', gameId, x, y);
                dispatch(setSelectedPosition({ x, y }));
            }
        }
    };

    return { board, possibleMoves, handleSquareClick };
};