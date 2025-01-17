import React from "react";
import styles from "./Board.module.css";
import { pieceImages } from "../../types";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";
import { CurrentPlayerInfo } from "../Player";

interface BoardProps {
    onSquareClick: (x: number, y: number) => Promise<void>;
}

export const Board: React.FC<BoardProps> = ({ onSquareClick }) => {
    const { board, possibleMoves, currentPlayerId } = useSelector((state: RootState) => state.chess);

    const renderSquare = (x: number, y: number) => {
        const isPossibleMove = possibleMoves.some((move) => move.x === x && move.y === y);
        const piece = board[x][y] as keyof typeof pieceImages;

        return (
            <div
                key={`${x},${y}`}
                className={`${styles.square} ${isPossibleMove ? styles.possibleMove : ''}`}
                onClick={() => {
                    if (currentPlayerId) {
                        onSquareClick(x, y);
                    } else {
                        console.log("No player is currently active.");
                    }
                }}
            >
                {piece && (
                    <img
                        src={pieceImages[piece]}
                        alt={piece}
                        className={styles.piece}
                    />
                )}
            </div>
        );
    };

    return (
        <div>
            <CurrentPlayerInfo /> {/* Tampilkan informasi pemain yang sedang giliran */}
            <div className={styles.board}>
                {Array(8)
                    .fill(null)
                    .map((_, x) => (
                        <div key={x} className={styles.row}>
                            {Array(8)
                                .fill(null)
                                .map((_, y) => renderSquare(x, y))}
                        </div>
                    ))}
            </div>
        </div>
    );
};