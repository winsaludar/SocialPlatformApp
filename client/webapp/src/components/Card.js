import Image from "next/image";

export default function AppListItem({
  imageSrc,
  imageWidth,
  imageHeight,
  title,
  description,
  onClick,
  styles,
  hasButton,
  buttonTitle,
  buttonOnClick,
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
        {hasButton && buttonTitle && buttonOnClick && (
          <button
            type="button"
            className={styles.cardButton}
            onClick={buttonOnClick}
          >
            {buttonTitle}
          </button>
        )}
      </div>
    </div>
  );
}
