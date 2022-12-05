import styles from "../../styles/Loader.module.css";

export default function Loader({ height }) {
  return (
    <div className={styles.container} style={{ height: height }}>
      <div className={styles.area}>
        <span></span>
        <span></span>
      </div>
    </div>
  );
}
