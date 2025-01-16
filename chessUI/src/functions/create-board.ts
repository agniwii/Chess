class Cell {
    pos: string;
    piece: string;

    constructor(pos: string, piece: string) {
        this.pos = pos;
        this.piece = piece;
    }
}

const range = (n: number): number[] => {
    return Array.from({ length: n }, (_, i) => i);
}

export const createBoard = (fenString: string): Cell[] => {
    const fen = fenString.split(' ')[0];
    const fenPieces = fen.split('/').join('').split('');

    let pieces: string[] = Array.from(fenPieces);

    Array.from(fenPieces).forEach((item, index) => {
        if (isFinite(Number(item))) {
            pieces.splice(index, 1, ...Array(Number(item)).fill(''));
        }
    })
    pieces = pieces.flat() as string[];

    const rows = range(8)
        .map((n) => n.toString())
        .reverse();

    const cols = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

    const cells: string[] = [];

    for(let i = 0;i<rows.length;i++) {
        const row = rows[i];
        for(let j = 0;j<cols.length;j++) {
            const col = cols[j];
            cells.push(col+row);
        }
    }
    const board: Cell[] = [];
    for(let i=0;i<cells.length;i++){
        const cell = cells[i];
        const piece = pieces[i];
        board.push(new Cell(cell, piece));
    }
    return board;
} 