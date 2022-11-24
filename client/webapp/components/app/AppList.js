import AppListItem from "./AppListItem";
import styles from "../../styles/AppList.module.css";

export default function AppList({ title }) {
  return (
    <>
      <div className={styles.grid}>
        {[...Array(12)].map((x, i) => {
          return (
            <AppListItem
              key={i}
              imageSrc="https://unsplash.it/600/400"
              imageWidth={600}
              imageHeight={400}
              title={`App Title ${i + 1}`}
              description={`App Description ${i + 1}`}
            />
          );
        })}
      </div>
    </>
  );
}
