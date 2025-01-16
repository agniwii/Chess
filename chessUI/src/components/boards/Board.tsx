import React from "react";
import { useChessGame } from "./useChessGame";
import styles from "./Board.module.css";
import { pieceImages } from "../../types";
import { useSelector } from "react-redux";
import { RootState } from "../../store/store";

interface BoardProps {
    gameId: string;
}

export const Board: React.FC<BoardProps> = ({gameId}) => {
  const {board, possibleMoves, handleSquareClick} = useChessGame(gameId);
  const currentPlayerId = useSelector((state: RootState) => state.chess.currentPlayerId);
  const renderSquare = (x: number, y: number) =>{
    const isPossibleMove = possibleMoves.some((move) => move.x === x && move.y === y);
    const piece = board[x][y] as keyof typeof pieceImages;
    return(
      <div
        key={`${x},${y}`}
        className={`${styles.square} ${isPossibleMove ? styles.possibleMove : ''}`}
        onClick={() => handleSquareClick(x,y)}
        >
          {piece &&(
            <img
            src={pieceImages[piece]}
            alt={piece}
            className={styles.piece}
            />
          )}
      </div>
    );
  }
  return(
    <div className={styles.board}>
      {Array(8)
        .fill(null)
        .map((_,x) =>(
          <div key={x} className={styles.row}>
            {Array(8)
              .fill(null)
              .map((_,y) => renderSquare(x,y))}
          </div>
        ))}
    </div>
  )
}