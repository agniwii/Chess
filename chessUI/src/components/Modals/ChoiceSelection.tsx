import React from 'react';
import styles from './Modals.module.css';

interface ChoiceSelectionProps {
    onChoiceClick: (choice: string) => void; // Fungsi untuk menangani klik pilihan
}

export const ChoiceSelection: React.FC<ChoiceSelectionProps> = ({ onChoiceClick }) => {
    return (
        <>
            <div className={styles.Cirle}>
                <img
                    src="/icon-rock.svg"
                    onClick={() => onChoiceClick("Rock")}
                    alt="Rock"
                    width={100}
                    height={100}
                    className={styles.Rock}
                />
                <div className={styles.paperScissors}>
                    <img
                        src="/icon-paper.svg"
                        onClick={() => onChoiceClick("Paper")}
                        alt="Paper"
                        width={100}
                        height={100}
                        className={styles.Paper}
                    />
                    <img
                        src="/icon-scissors.svg"
                        onClick={() => onChoiceClick("Scissors")}
                        alt="Scissors"
                        width={100}
                        height={100}
                        className={styles.Scissors}
                    />
                </div>
            </div>
        </>
    );
};