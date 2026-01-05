import { useState, createContext, useContext } from 'react';
import './ConfirmModal.css';

const ConfirmContext = createContext(null);

export function ConfirmProvider({ children }) {
    const [confirmState, setConfirmState] = useState({
        isOpen: false,
        title: '',
        message: '',
        type: 'warning', // 'warning', 'danger', 'info'
        onConfirm: null,
        confirmText: 'Confirm',
        cancelText: 'Cancel'
    });

    const confirm = (options) => {
        return new Promise((resolve) => {
            setConfirmState({
                isOpen: true,
                title: options.title || 'Confirm',
                message: options.message || 'Are you sure?',
                type: options.type || 'warning',
                confirmText: options.confirmText || 'Confirm',
                cancelText: options.cancelText || 'Cancel',
                onConfirm: () => {
                    setConfirmState(prev => ({ ...prev, isOpen: false }));
                    resolve(true);
                }
            });
        });
    };

    const handleCancel = () => {
        setConfirmState(prev => ({ ...prev, isOpen: false }));
    };

    return (
        <ConfirmContext.Provider value={{ confirm }}>
            {children}
            {confirmState.isOpen && (
                <div className="confirm-overlay" onClick={handleCancel}>
                    <div className={`confirm-modal ${confirmState.type}`} onClick={e => e.stopPropagation()}>
                        <div className="confirm-icon">
                            {confirmState.type === 'danger' && '!'}
                            {confirmState.type === 'warning' && '!'}
                            {confirmState.type === 'info' && '?'}
                        </div>
                        <h3 className="confirm-title">{confirmState.title}</h3>
                        <p className="confirm-message">{confirmState.message}</p>
                        <div className="confirm-actions">
                            <button className="btn-cancel" onClick={handleCancel}>
                                {confirmState.cancelText}
                            </button>
                            <button
                                className={`btn-confirm ${confirmState.type}`}
                                onClick={confirmState.onConfirm}
                            >
                                {confirmState.confirmText}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </ConfirmContext.Provider>
    );
}

export const useConfirm = () => {
    const context = useContext(ConfirmContext);
    if (!context) {
        throw new Error('useConfirm must be used within a ConfirmProvider');
    }
    return context;
};
