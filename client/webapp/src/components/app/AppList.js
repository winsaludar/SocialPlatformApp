import styles from "../../../styles/AppContainer.module.css";
import AppListItem from "./AppListItem";

export default function AppList({ onItemClick }) {
  return (
    <>
      <div className={styles.grid}>
        {[...Array(12)].map((x, i) => {
          return (
            <AppListItem
              key={i}
              imageSrc={`/images/placeholder/ali-${i + 1}.jpg`}
              imageWidth={1680}
              imageHeight={1050}
              title={`App Title ${i + 1}`}
              description="Nullam posuere nibh augue, nec sagittis ex eleifend sed. Curabitur tristique porta consequat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
              onClick={onItemClick}
            />
          );
        })}
      </div>
    </>
  );
}