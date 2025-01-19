import React, { useState } from 'react';
import { useSignalR } from '../../hooks/signalRContext'; // Import the context
import styles from './joinGame.module.css';

export const JoinGame: React.FC = () => {
    const { joinRoom, setPlayerName } = useSignalR(); // Use the context
    const [nameInput, setNameInput] = useState('');

    const handleJoin = () => {
        if (nameInput.trim()) {
            setPlayerName(nameInput); // Set the player's name in the context
            joinRoom("test", nameInput); // Replace "test" with your game ID logic
        } else {
            alert("Please enter a valid player name");
        }
    };

    return (
        <div className={styles.JoinGame}>
            <input
                type="text"
                placeholder="Enter Player Name"
                value={nameInput}
                onChange={(e) => setNameInput(e.target.value)}
            />
            <button className={styles.button} onClick={handleJoin}>
                Join Game
            </button>
        </div>
    );
};