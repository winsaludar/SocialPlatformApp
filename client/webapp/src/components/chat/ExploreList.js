import styles from "../../../styles/ChatComponent.module.css";
import Card from "../Card";

export default function ExploreList() {
  return (
    <main className={styles.serverList}>
      {[...Array(30)].map((x, i) => {
        return (
          <Card
            key={i}
            imageSrc={`/images/placeholder/ali-${(i % 12) + 1}.jpg`}
            imageWidth={300}
            imageHeight={300}
            title={`Server ${i + 1}`}
            description="Nullam posuere nibh augue, nec sagittis ex eleifend sed. Curabitur tristique porta consequat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
            onClick={() => {}}
            styles={styles}
          />
        );
      })}
    </main>
  );
}