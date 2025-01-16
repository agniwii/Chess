
import React from "react";

import styles from "./joinGame.module.css";

interface JoinGameProps {
    onJoin: (playerName: string) => void;
}
export const JoinGame: React.FC<JoinGameProps> = ({onJoin}) => {
    const [playerName, setPlayerName] = React.useState("");
    const handleJoin = () =>{
        if(playerName.trim()){
            onJoin(playerName);
        }else{
            alert("Please enter a valid game code and player name");
        }
    };
    return (
        <div className={styles.JoinGame}>
            <input
            type="text"
            placeholder="Enter Player Name"
            value={playerName}
            onChange={(e) => setPlayerName(e.target.value)}
            />
            <button className={styles.button} onClick={handleJoin}>Join Game</button>
        </div>
    );
};

