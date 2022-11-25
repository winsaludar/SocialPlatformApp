import Image from "next/image";
import styles from "../../../styles/AppContainer.module.css";

export default function AppListItem({
  imageSrc,
  imageWidth,
  imageHeight,
  title,
  description,
  onClick,
}) {
  return (
    <div
      className={styles.card}
      onClick={() => onClick(title, description, imageSrc)}
    >
      <div className={styles.cardImage}>
        <Image src={imageSrc} width={imageWidth} height={imageHeight} alt="" />
      </div>
      <div className={styles.cardContent}>
        <h2 className={styles.cardTitle}>{title}</h2>
        <p className={styles.cardDescription}>{description}</p>
        <button type="button" className={styles.cardButton}>
          Open App
        </button>
      </div>
    </div>
  );
}
