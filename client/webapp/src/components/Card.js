import Image from "next/image";

import utilStyles from "../../styles/utils.module.css";

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
  isDisabled,
  customImageLoader,
}) {
  return (
    <div
      className={`${styles.card} ${isDisabled ? utilStyles.cardDisabled : ""}`}
    >
      {imageSrc && imageWidth && imageHeight && (
        <div
          className={styles.cardImage}
          onClick={
            !isDisabled ? () => onClick(title, description, imageSrc) : null
          }
        >
          <Image
            loader={customImageLoader}
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
