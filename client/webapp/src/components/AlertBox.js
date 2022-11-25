import styles from "../../styles/AlertBox.module.css";

export default function AlertBox({ type, messages, onCloseButtonClick }) {
  let alertTypeClass = "";
  if (type === "success") alertTypeClass = styles.alertSuccess;
  if (type === "error") alertTypeClass = styles.alertError;

  return (
    <div className={`${styles.alert} ${alertTypeClass}`}>
      <button
        type="button"
        className={styles.closeButton}
        onClick={onCloseButtonClick}
      >
        &times;
      </button>

      {messages && (
        <ul>
          {messages.map((item) => (
            <li key={item} className={styles.text}>
              {item}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
