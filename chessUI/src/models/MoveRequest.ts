import { Position } from "./Position";

export interface MoveRequest {
    gameId: string;
    playerId: string;
    from: Position;
    to: Position;
}