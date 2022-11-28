import styles from "../../../styles/AppComponent.module.css";
import Card from "../Card";
import { useRouter } from "next/router";

export default function AppList({ onItemClick }) {
  const route = useRouter(null);

  return (
    <>
      <div className={styles.grid}>
        <Card
          imageSrc={`/images/placeholder/ali-1.jpg`}
          imageWidth={1680}
          imageHeight={1050}
          title={`Chat`}
          description="Nullam posuere nibh augue, nec sagittis ex eleifend sed. Curabitur tristique porta consequat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
          onClick={onItemClick}
          styles={styles}
          hasButton={true}
          buttonTitle="Open App"
          buttonOnClick={() => {
            route.push("/chat");
          }}
        />

        {[...Array(11)].map((x, i) => {
          return (
            <Card
              key={i}
              imageSrc={`/images/placeholder/ali-${i + 1}.jpg`}
              imageWidth={1680}
              imageHeight={1050}
              title={`App Title ${i + 1}`}
              description="Nullam posuere nibh augue, nec sagittis ex eleifend sed. Curabitur tristique porta consequat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
              onClick={onItemClick}
              styles={styles}
              hasButton={true}
              buttonTitle="Open App"
              buttonOnClick={() => {}}
              isDisabled={true}
            />
          );
        })}
      </div>
    </>
  );
}
