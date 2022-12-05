import styles from "../../../styles/ChatComponent.module.css";
import Card from "../Card";
import Loader from "../Loader";

const loader = ({ src, width, quality }) => {
  return src;
};

export default function ExploreList({ servers, showLoader }) {
  return (
    <main className={styles.serverList}>
      {showLoader && <Loader height="50vh" />}
      {/* <Loader height="50vh" /> */}

      {Array.isArray(servers) &&
        servers.map((server) => {
          return (
            <Card
              key={server.id}
              imageSrc={decodeURIComponent(server.thumbnail)}
              imageWidth={640}
              imageHeight={360}
              title={server.name}
              description={server.shortDescription}
              onClick={() => {}}
              styles={styles}
              customImageLoader={loader}
            />
          );
        })}
    </main>
  );
}
