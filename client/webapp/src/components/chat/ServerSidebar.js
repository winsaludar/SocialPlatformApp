import styles from "../../../styles/ChatComponent.module.css";

export default function ServerSidebar({ serverName, channels }) {
  return (
    <aside className={styles.sidebar}>
      <h2>{serverName}</h2>

      <div></div>
    </aside>
  );
}
