import styles from "../../../styles/ChatComponent.module.css";

export default function ChatHeader() {
  return (
    <header className={styles.header}>
      <div>
        <h1>Find your community</h1>
        <p>
          From gaming, to music, to learning, there is always a place for you
        </p>
        <form className={styles.headerForm}>
          <input
            type="text"
            placeholder="Explore communities"
            autoComplete="off"
          />
        </form>
      </div>
    </header>
  );
}
