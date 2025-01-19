import { useEffect, useState } from 'react';
import { useSignalR } from './hooks/signalRContext'; // Ensure this path is correct
import { Modals } from './components/Modals';
import { JoinGame } from './pages/joinGame';
import './App.css';

function App() {
    const { connection, playerId, gameStatus } = useSignalR();
    const [isJoining, setIsJoining] = useState(false);

    useEffect(() => {
        const checkPlayer = async () => {
            if (connection) {
                if (playerId) {
                    setIsJoining(true);
                }
            }
        };
        console.log('gameStatus:', gameStatus);
        checkPlayer();
    }, [connection, playerId]);

    return (
        <>
            <div className='Container'>
                <h1>Chess</h1>
                {!isJoining && <JoinGame />}
            </div>

            {connection && isJoining && (
                <Modals 
                    isOpen={isJoining}
                    forceHideChildren={gameStatus === 'RPSStarted'}
                >
                    {gameStatus === 'Loading' && (
                        <div className='Loading-Container'>
                            <h2>Waiting for player 2</h2>
                            <div className="loader"></div>
                        </div>
                    )}

                    {gameStatus === 'RPSStarted' && null}

                    {gameStatus === 'ChessStarted' && (
                        <div>
                            <h2>Chess game is starting!</h2>
                        </div>
                    )}
                </Modals>
            )}
        </>
    );
}

export default App;