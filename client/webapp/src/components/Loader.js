import styles from "../../styles/Loader.module.css";

export default function Loader() {
  return (
    <div className={styles.container}>
      <div className={styles.area}>
        <span></span>
        <span></span>
      </div>
    </div>
  );
}
