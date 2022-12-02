import styles from "../../../styles/ChatComponent.module.css";

export default function Sidebar() {
  return (
    <aside className={styles.sidebar}>
      <h2>Discover</h2>

      <div>
        <button className={styles.buttonActive} type="button">
          Home
        </button>
        <button type="button">Gaming</button>
        <button type="button">Education</button>
        <button type="button">Science & Technology</button>
        <button type="button">Entertainment</button>
        <button type="button">Others</button>
      </div>
    </aside>
  );
}