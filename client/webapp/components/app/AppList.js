import Head from "next/head";
import AppListItem from "./AppListItem";
import styles from "../../styles/AppList.module.css";

export default function AppList({ title }) {
  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>

      <div className={styles.container}>
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
      </div>
    </>
  );
}
