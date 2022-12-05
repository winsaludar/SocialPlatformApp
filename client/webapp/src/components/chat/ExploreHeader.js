import styles from "../../../styles/ChatComponent.module.css";

export default function ExploreHeader({ onTextChange }) {
  return (
    <header className={styles.header}>
      <div>
        <h1>Find your community</h1>
        <p>
          From gaming, to music, to learning, there is always a place for you
        </p>
        <form
          className={styles.headerForm}
          onSubmit={(e) => e.preventDefault()}
        >
          <input
            type="text"
            placeholder="Explore communities"
            autoComplete="off"
            onChange={onTextChange}
          />
        </form>
      </div>
    </header>
  );
}
