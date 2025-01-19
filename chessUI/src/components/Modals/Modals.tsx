import React, { useEffect, useState } from 'react';
import styles from './Modals.module.css';
import { useSignalR } from '../../hooks/signalRContext'; // Import the context
import { ChoiceSelection } from './ChoiceSelection';

interface ModalsProps {
    isOpen: boolean;
    children?: React.ReactNode;
    forceHideChildren?: boolean;  // Tambahkan prop baru
}

export const Modals: React.FC<ModalsProps> = ({ isOpen, children, forceHideChildren = false }) => {
    const { connection, playerId, gameStatus, submitRPSChoice } = useSignalR();
    const [result, setResult] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleOverlayClick = async (choice: string) => {
        if (connection && playerId) {
            setIsLoading(true); // Enable loading
            try {
                submitRPSChoice("test", playerId, choice); // Use the gameId from context if available
            } catch (error) {
                console.error("Error making choice:", error);
                setIsLoading(false); // Disable loading if there's an error
            }
        }
    };

    useEffect(() => {
        if (connection) {
            // Listen for RPSResult messages from the server
            connection.on("RPSResult", (message: string) => {
                console.log("RPSResult received:", message); // Log the result
                setResult(message);
                setIsLoading(false); // Disable loading when the result is received
            });
        }

        return () => {
            if (connection) {
                connection.off("RPSResult"); // Clean up the event listener
            }
        };
    }, [connection]);

    if (!isOpen) return null;

    // Jika gameStatus null, tidak merender children
    if (gameStatus === null) {
        return null;
    }

    return (
        <div className={styles.overlay}>
            <div className={styles.modal}>
                <div className={styles.modalContent}>
                    {!forceHideChildren && children ? children : (
                        isLoading ? (
                            <>
                                <div className={styles.loader}></div>
                                <h2>Waiting for opponent...</h2>
                            </>
                        ) : (
                            result === null || result === "Draw" ? (
                                <ChoiceSelection onChoiceClick={handleOverlayClick} />
                            ) : (
                                <>
                                    <h2>{result}</h2>
                                </>
                            )
                        )
                    )}
                </div>
            </div>
        </div>
    );
};