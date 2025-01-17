import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../store/store';

export const CurrentPlayerInfo: React.FC = () => {
    const currentPlayerId = useSelector((state: RootState) => state.chess.currentPlayerId);

    return (
        <div>
            <h2>Current Player: {currentPlayerId}</h2>
        </div>
    );
};
