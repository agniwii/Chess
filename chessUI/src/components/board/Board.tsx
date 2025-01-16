import React from "react";
import styles from "./Board.module.css";
import Cell from "../cells";

interface Cells {
    pos: string;
    piece: string;
}

interface BoardProps {
    cells: Cells[];
}

export const Board: React.FC<BoardProps> = ({ cells }) => {
    return (
        <div className={styles.Board}>
            {cells.map((cell, index) => (
                console.log(cell),
                <Cell cell={cell} key={cell.pos} index={index} /> 
            ))}
        </div>
    );
};

