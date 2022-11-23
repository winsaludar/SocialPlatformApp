import Image from "next/image";
import styles from "../../styles/AppList.module.css";
import utilStyles from "../../styles/utils.module.css";

function loader({ src, width, quality }) {
  return `${src}?w=${width}&q=${quality || 75}`;
}

export default function AppListItem({
  imageSrc,
  imageWidth,
  imageHeight,
  title,
  description,
}) {
  return (
    <div className={`${styles.card} ${utilStyles.stacked}`}>
      <Image
        loader={loader}
        src={imageSrc}
        className={styles.cardImage}
        width={imageWidth}
        height={imageHeight}
        alt=""
      />
      <div className={styles.cardContent}>
        <h2 className={styles.cardTitle}>{title}</h2>
        <p className={styles.cardDescription}>{description}</p>
      </div>
    </div>
  );
}
