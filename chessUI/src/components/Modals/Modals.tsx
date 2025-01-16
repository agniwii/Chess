import React, { useEffect, useState } from 'react';
import styles from './Modals.module.css';
import { invokeServerMethod, onServerMethod } from '../../utilities/network';
import { ChoiceSelection } from './ChoiceSelection';

interface ModalsProps {
    connectionId: signalR.HubConnection;
    gameId: string;
    isOpen: boolean;
    children: React.ReactNode;
}

export const Modals: React.FC<ModalsProps> = ({ connectionId, gameId, isOpen, children }) => {
    const [result, setResult] = React.useState<string | null>("");
    const [isLoading, setIsLoading] = useState(false); // State untuk loading

    const handleOverlayClick = async (choice: string) => {
        if (connectionId) {
            setIsLoading(true); // Aktifkan loading
            try {
                await invokeServerMethod(connectionId, "MakeChoice", gameId, choice);
            } catch (error) {
                console.error("Error making choice:", error);
                setIsLoading(false); // Matikan loading jika terjadi error
            }
        }
    };
    useEffect(() => {
        onServerMethod(connectionId, "RPSResult", (...args: unknown[]) => {
            const message = args[0] as string;
            setResult(message);
            setIsLoading(false); 
        });
    }, [connectionId]);
    if (!isOpen) return null;
    return (
        <div className={styles.overlay}>
            <div className={styles.modal}>
                <div className={styles.modalContent}>
                    {children ?? (
                        isLoading ? (
                            <>
                                <div className={styles.loader}></div>
                                <h2>Waiting for opponent...</h2>
                            </>
                        ) : (
                            result == "" || result == "Draw" ? (
                                <ChoiceSelection onChoiceClick={handleOverlayClick}/>
                            ) : (
                                <>
                                    <h2>{result}</h2>
                                </>

                            )
                        )
                    )
                    }
                </div>
            </div>
        </div>
    );
}