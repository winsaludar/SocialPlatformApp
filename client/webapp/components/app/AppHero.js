import styles from "../../styles/AppContainer.module.css";

export default function AppHero() {
  return (
    <div className={styles.hero}>
      <div className={styles.heroContainer}>
        <h1>Some really interesting text here</h1>
        <p>
          Donec eleifend erat lacus, ac porttitor purus ultrices at. Proin
          sagittis interdum ex in sollicitudin. Integer et nibh fringilla,
          vehicula velit quis, blandit lorem. Morbi id tortor ante. Quisque in
          est egestas, posuere magna feugiat, iaculis purus.
        </p>
      </div>
    </div>
  );
}
