import Head from "next/head";
import Image from "next/image";
import styles from "../../styles/AppList.module.css";

function loader({ src, width, quality }) {
  return `${src}?w=${width}&q=${quality || 75}`;
}

export default function AppList({ title }) {
  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>

      <div className={styles.container}>
        <div className={styles.grid}>
          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 1</h2>
              <p className={styles.cardDescription}>App Description 1</p>
            </div>
          </div>

          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 2</h2>
              <p className={styles.cardDescription}>App Description 2</p>
            </div>
          </div>

          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 3</h2>
              <p className={styles.cardDescription}>App Description 3</p>
            </div>
          </div>

          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 4</h2>
              <p className={styles.cardDescription}>App Description 4</p>
            </div>
          </div>

          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 5</h2>
              <p className={styles.cardDescription}>App Description 5</p>
            </div>
          </div>

          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 6</h2>
              <p className={styles.cardDescription}>App Description 6</p>
            </div>
          </div>

          <div className={styles.card}>
            <Image
              loader={loader}
              src="https://unsplash.it/600/400"
              className={styles.cardImage}
              width={600}
              height={400}
              alt=""
            />
            <div className={styles.cardContent}>
              <h2 className={styles.cardTitle}>App Title 7</h2>
              <p className={styles.cardDescription}>App Description 7</p>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
