import Image from "next/image";
import utilStyles from "../../styles/utils.module.css";

export default function AppListItem({
  cssBacgroundImage,
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
  isDisabled,
}) {
  return (
    <div
      className={`${styles.card} ${isDisabled ? utilStyles.cardDisabled : ""}`}
      onClick={!isDisabled ? () => onClick(title, description, imageSrc) : null}
    >
      {imageSrc && imageWidth && imageHeight && (
        <div className={styles.cardImage}>
          <Image
            src={imageSrc}
            width={imageWidth}
            height={imageHeight}
            alt=""
          />
        </div>
      )}

      <div className={styles.cardContent}>
        <h2 className={styles.cardTitle}>{title}</h2>
        <p className={styles.cardDescription}>{description}</p>
        {hasButton && buttonTitle && buttonOnClick && (
          <button
            type="button"
            className={styles.cardButton}
            onClick={
              !isDisabled
                ? (e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    buttonOnClick();
                  }
                : null
            }
          >
            {buttonTitle}
          </button>
        )}
      </div>
    </div>
  );
}
