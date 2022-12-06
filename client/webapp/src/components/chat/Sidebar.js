import styles from "../../../styles/ChatComponent.module.css";

export default function Sidebar({ selectedCategory, onButtonClick }) {
  return (
    <aside className={styles.sidebar}>
      <h2>Discover</h2>

      <div>
        <button
          className={selectedCategory === "home" ? styles.buttonActive : null}
          type="button"
          onClick={() => onButtonClick("home")}
        >
          Home
        </button>
        <button
          className={selectedCategory === "gaming" ? styles.buttonActive : null}
          type="button"
          onClick={() => onButtonClick("gaming")}
        >
          Gaming
        </button>
        <button
          className={
            selectedCategory === "education" ? styles.buttonActive : null
          }
          type="button"
          onClick={() => onButtonClick("education")}
        >
          Education
        </button>
        <button
          className={
            selectedCategory === "science" ? styles.buttonActive : null
          }
          type="button"
          onClick={() => onButtonClick("science")}
        >
          Science & Technology
        </button>
        <button
          className={
            selectedCategory === "entertainment" ? styles.buttonActive : null
          }
          type="button"
          onClick={() => onButtonClick("entertainment")}
        >
          Entertainment
        </button>
        <button
          className={selectedCategory === "others" ? styles.buttonActive : null}
          type="button"
          onClick={() => onButtonClick("others")}
        >
          Others
        </button>
      </div>
    </aside>
  );
}
