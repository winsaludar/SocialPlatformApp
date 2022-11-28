import styles from "../../../styles/AppComponent.module.css";

export default function AppHero({ data }) {
  return (
    <div
      className={styles.hero}
      style={{ backgroundImage: ` url(${data.backgroundImage})` }}
    >
      <div className={styles.heroContainer}>
        <h1>{data.title}</h1>
        <p>{data.description}</p>
      </div>
    </div>
  );
}
