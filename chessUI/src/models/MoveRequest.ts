import { Position } from "./Position";

export interface MoveRequest {
    gameId: string;
    from: Position;
    to: Position;
}