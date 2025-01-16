import React from 'react';
import{ piecesImages} from './pieces';
import styles from './Piece.module.css';

// Definisikan tipe untuk props komponen `Piece`
interface PieceProps {
    name: string;
    pos: string;
}

const Piece: React.FC<PieceProps> = ({ name, pos }) => {
    let image: string;
    try {
        image = piecesImages[name];
        console.log(pos);
    } catch (error) {
        console.log(`Piece ${name} not found, using pawn instead`);
        console.log(error);
        image = piecesImages['p'];
    }

    return (
        <img
            className={styles.Piece}
            src={image}
            alt=""
            draggable={true}
        />
    );
};


export default Piece;