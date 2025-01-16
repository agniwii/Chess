import React from "react";
import styles from "./Cell.module.css";
import { isLightSquare } from '../../functions';
import Piece from "../piece";


interface Cell {
    pos: string;
    piece: string;
}

interface CellProps {
    cell: Cell; // Terima `cell` sebagai prop, bukan `cells`
    index: number;
}

const Cell: React.FC<CellProps> = ({ cell, index }) => {
    const position = [
        cell.pos.charCodeAt(0) - 'a'.charCodeAt(0) + 1,
        parseInt(cell.pos[1], 10),
    ];

    const light = isLightSquare(position, index);

    return (
        <div className={`${styles.Cell} ${light ? styles.Light : styles.Dark}`}>
            <Piece name={cell.piece} pos={cell.pos} />
        </div>
    );
};

export default Cell;