import Image from "next/image";

import styles from "../../styles/AppContainer.module.css";
import utilStyles from "../../styles/utils.module.css";
import { unsplashLoader } from "../../utils/imageUtils.js";

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
        loader={unsplashLoader}
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
